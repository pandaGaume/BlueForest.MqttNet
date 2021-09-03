using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Subscribing;
using MQTTnet.Client.Unsubscribing;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BlueForest.MqttNet.Dataflow
{
    public class MqttSourceBlock<T> : IWithBroker, ISourceBlock<IEnumerable<T>>, IDisposable
    {
        IMqttSourceBlockOptions<T> _options;
        ITargetBlock<MqttApplicationMessageReceivedEventArgs> _target;
        IPropagatorBlock<MqttApplicationMessageReceivedEventArgs, IEnumerable<T>> _decoder;
        ISourceBlock<IEnumerable<T>> _source;
        string[][] _subscribed;
        private bool _disposed = false;
        public IManagedMqttClient Broker => _options.ManagedClient;

        public MqttSourceBlock(IMqttSourceBlockOptions<T> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            if (options.Codec == null) throw new ArgumentNullException(nameof(options.Codec));
            if (options.ManagedClient== null ) throw new ArgumentNullException(nameof(options.ManagedClient));
            // make sure our complete call gets propagated throughout the whole pipeline
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            var inputBuffer = new BufferBlock<MqttApplicationMessageReceivedEventArgs>(_options.SourceOptions ?? new DataflowBlockOptions());
            var decoder = new TransformBlock<MqttApplicationMessageReceivedEventArgs, IEnumerable<T>>(DecodeAsync, _options.DecoderOptions ?? new ExecutionDataflowBlockOptions());
            var outputBuffer = new BufferBlock<IEnumerable<T>>(_options.TargetOptions ?? new DataflowBlockOptions());

            // apply Filter (default is filtering topics if enabled in options.FilteringTopic)
            inputBuffer.LinkTo(decoder, linkOptions, Filter);
            // add null target for filtered values
            inputBuffer.LinkTo(DataflowBlock.NullTarget<MqttApplicationMessageReceivedEventArgs>());
            // filter default value
            decoder.LinkTo(outputBuffer, linkOptions);

            _target = inputBuffer;
            _decoder = decoder;
            _source = outputBuffer;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            var client = Broker;
            if (client != null)
            {
                client.OnConnected += OnConnectedAsync;
                await client.StartAsync(cancellationToken);
            }
        }
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            var client = Broker;
            if (client != null)
            {
                var optionsBuilder = new MqttClientUnsubscribeOptionsBuilder();
                foreach (var tf in _options.Topics)
                {
                    optionsBuilder.WithTopicFilter(tf);
                }
                try
                {
                    _subscribed = null;
                    var unsubscribeResult = await client.Client.UnsubscribeAsync(optionsBuilder.Build(), CancellationToken.None);
                    foreach (var r in unsubscribeResult.Items)
                    {
                        if (r.ReasonCode == MqttClientUnsubscribeResultCode.Success)
                        {
                        }
                        else
                        {
                        }
                    }
                }
                catch
                {
                }

                client.OnConnected -= OnConnectedAsync;
                client.OnDisconnected -= OnDisconnectedAsync;
                client.OnMessage -= OnMessageAsync;

                await client.StopAsync(cancellationToken);
            }
        }
        protected virtual Task OnDisconnectedAsync(object sender, MqttClientDisconnectedEventArgs args)
        {
            var client = Broker;
            if (client != null)
            {
                client.OnDisconnected -= OnDisconnectedAsync;
                client.OnMessage -= OnMessageAsync;
                client.OnConnected += OnConnectedAsync;
            }
            return Task.CompletedTask;
        }
        protected virtual async Task OnConnectedAsync(object sender, MqttClientConnectedEventArgs args)
        {
            var client = Broker;
            if (client != null)
            {
                client.OnConnected -= OnConnectedAsync;
                client.OnDisconnected += OnDisconnectedAsync;
                client.OnMessage += OnMessageAsync;
                var optionsBuilder = new MqttClientSubscribeOptionsBuilder();
                var topics = _options.Topics;
                foreach (var tf in topics)
                {
                    optionsBuilder.WithTopicFilter(tf);
                }
                try
                {
                    var subscribeResult = await client.Client.SubscribeAsync(optionsBuilder.Build(), CancellationToken.None);
                    _subscribed = topics.Select(f => f.Topic.Split(MqttProtocol.TOPIC_SEPARATOR)).ToArray();
                    foreach (var r in subscribeResult.Items)
                    {
                        if (r.ResultCode <= MqttClientSubscribeResultCode.GrantedQoS2)
                        {
                        }
                        else
                        {
                        }
                    }
                }
                catch(Exception e)
                {
                }
            }
        }
        protected virtual async Task OnMessageAsync(object sender, MqttApplicationMessageReceivedEventArgs args)
        {
            await _target.SendAsync(args);
        }
        protected virtual bool Filter(MqttApplicationMessageReceivedEventArgs args)
        {
            if (_options.FilteringTopics)
            {
                var parts = args.ApplicationMessage.Topic.Split(MqttProtocol.TOPIC_SEPARATOR);
                for (int i = 0; i != _subscribed.Length; i++)
                {
                    if (_subscribed[i].Match(parts))
                    {
                        if (i != 0)
                        {
                            // bubble up the topic up to speed up the search
                            var tmp = _subscribed[i - 1];
                            _subscribed[i - 1] = _subscribed[i];
                            _subscribed[i] = tmp;
                        }
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
        protected virtual byte[] GetPayload(MqttApplicationMessageReceivedEventArgs args)
        {
            // the payload MAY be compressed.
            var m = args.ApplicationMessage.Payload;
            try
            {
                if (m.Length > 2)
                {
                    if (m[0] == 0x1f && m[1] == 0x8b)
                    {
                        using (MemoryStream data = new MemoryStream(m))
                        {
                            using (MemoryStream decompressedData = new MemoryStream())
                            {
                                using (GZipStream stream = new GZipStream(data, CompressionMode.Decompress))
                                {
                                    stream.CopyTo(decompressedData);
                                }
                                return decompressedData.ToArray();
                            }
                        }
                    }
                }
            }
            catch
            {
                // something went wrong into decompress
            }
            return m;
        }
        protected async virtual Task<IEnumerable<T>> DecodeAsync(MqttApplicationMessageReceivedEventArgs args)
        {
            try
            {
               return await _options.Codec.GetMessageAsync(GetPayload(args));
            }
            catch
            {
                return Enumerable.Empty<T>();
            }
        }
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
        #region ISourceBlock<T>
        public Task Completion => _source.Completion;
        public IEnumerable<T> ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<IEnumerable<T>> target, out bool messageConsumed) => _source.ConsumeMessage(messageHeader, target, out messageConsumed);
        public IDisposable LinkTo(ITargetBlock<IEnumerable<T>> target, DataflowLinkOptions linkOptions) => _source.LinkTo(target, linkOptions);
        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<IEnumerable<T>> target) => _source.ReleaseReservation(messageHeader, target);
        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<IEnumerable<T>> target) => _source.ReserveMessage(messageHeader, target);
        public void Complete() => _source.Complete();
        public void Fault(Exception exception) => _source.Fault(exception);
        #endregion
    }
}
