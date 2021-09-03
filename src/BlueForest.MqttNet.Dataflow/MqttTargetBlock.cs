using MQTTnet;
using MQTTnet.Client.Publishing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BlueForest.MqttNet.Dataflow
{
    using PAYLOAD = IEnumerable<MqttApplicationMessage>;
    public class MqttTargetBlock<T> : IWithBroker, ITargetBlock<IEnumerable<T>>, IDisposable
    {
        public static TimeSpan PublishRetryMaxDelayDefault = TimeSpan.FromSeconds(30);

        IMqttTargetBlockOptions<T> _options;
        ITargetBlock<IEnumerable<T>> _target;
        IPropagatorBlock<IEnumerable<T>, PAYLOAD> _encoder;
        ITargetBlock<PAYLOAD> _publisher;
        private bool _disposed = false;

        public IManagedMqttClient Broker => _options.ManagedClient;

        public MqttTargetBlock(IMqttTargetBlockOptions<T> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            if (options.Codec == null) throw new ArgumentNullException(nameof(options.Codec));
            if (options.ManagedClient == null) throw new ArgumentNullException(nameof(options.ManagedClient));

            // make sure our complete call gets propagated throughout the whole pipeline
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            var inputBuffer = new BufferBlock<IEnumerable<T>>(_options.InputOptions ?? new DataflowBlockOptions());
            var encoder = new TransformBlock<IEnumerable<T>, PAYLOAD>(EncodeAsync, _options.EncoderOptions ?? new ExecutionDataflowBlockOptions());
            var publisher = new ActionBlock<PAYLOAD>(PublishAsync,_options.PublishOptions ?? new ExecutionDataflowBlockOptions());

            inputBuffer.LinkTo(encoder, linkOptions);
            encoder.LinkTo(publisher, linkOptions);

            _target = inputBuffer;
            _encoder = encoder;
            _publisher = publisher;
        }

        protected async virtual Task<PAYLOAD> EncodeAsync(IEnumerable<T> args)
        {
            List<MqttApplicationMessage> l = new List<MqttApplicationMessage>(_options.Topics.Count);
            foreach (var m in args)
            {
                var payload = await _options.Codec.GetPayloadAsync(m);
                foreach (var t in _options.Topics)
                {
                    var newTopic = _options.Codec.GetTopic(t.Topic, m);
                    var builder = new MqttApplicationMessageBuilder().WithPayload(payload).WithTopic(newTopic).WithQualityOfServiceLevel(t.QualityOfServiceLevel);
                    l.Add(builder.Build());
                }
            }
            return l;
        }

        protected async virtual Task PublishAsync(PAYLOAD p)
        {
            foreach (var mess in p)
            {
                TimeSpan? ttw = null;
                int retryAttempt = 0;
                Random jitterer = new Random();
                do
                {
                    if (ttw != null)
                    {
                        await Task.Delay((TimeSpan)ttw);
                    }
                    
                    if (_options.ManagedClient.IsConnected())
                    {
                        try
                        {
                            var result = await _options.ManagedClient.PublishAsync(mess, CancellationToken.None);
                            if( result.ReasonCode != MqttClientPublishReasonCode.Success)
                            {
                                // TODO, post to dead lettering
                            }
                        }
                        catch
                        {
                            // TODO, log
                        }
                        // we ONLY re-process when connection is down 
                        break;
                    }
                    var maxSeconds = (_options?.PublishRetryMaxDelay ?? PublishRetryMaxDelayDefault).TotalSeconds;
                    // exponential backoff + jittering 
                    var seconds = Math.Min(maxSeconds, Math.Pow(2, ++retryAttempt));
                    ttw = TimeSpan.FromSeconds(seconds) + TimeSpan.FromMilliseconds(jitterer.Next(0, 100));
                } while (true);
            }
        }

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }
                _disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
        #region ITargetBlock
        public Task Completion => _target.Completion;
        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, IEnumerable<T> messageValue, ISourceBlock<IEnumerable<T>> source, bool consumeToAccept) 
        {
            return _options.ManagedClient.IsConnected() ? _target.OfferMessage(messageHeader, messageValue, source, consumeToAccept) : DataflowMessageStatus.Declined;
        }
        public void Complete() => _target.Complete();
        public void Fault(Exception exception) => _target.Fault(exception);
        #endregion
    }
}
