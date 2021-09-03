using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace BlueForest.MqttNet.Dataflow
{
    public class MqttTargetBlockOptionsBuilder<T>
    {
        MqttTargetBlockOptions<T> _options = new MqttTargetBlockOptions<T>();

        public MqttTargetBlockOptionsBuilder<T> WithMqttClient(IManagedMqttClient c)
        {
            _options.ManagedClient = c;
            return this;
        }
        public MqttTargetBlockOptionsBuilder<T> WithMqttClient(ManagedMqttClientBuilder cb)
        {
            _options.ManagedClient = cb.Build();
            return this;
        }
        public MqttTargetBlockOptionsBuilder<T> WithTopic(MqttTopicFilter topic)
        {
            _options.Topics.Add(topic);
            return this;
        }
        public MqttTargetBlockOptionsBuilder<T> WithTopic(MqttTopicFilterBuilder topicBuilder)
        {
            _options.Topics.Add(topicBuilder.Build());
            return this;
        }
        public MqttTargetBlockOptionsBuilder<T> WithTopics(IEnumerable<MqttTopicFilter> topics)
        {
            _options.Topics.AddRange(topics);
            return this;
        }
        public MqttTargetBlockOptionsBuilder<T> WithTopics(IEnumerable<MqttTopicFilterBuilder> topicBuilders)
        {
            _options.Topics.AddRange(topicBuilders.Select(b=>b.Build()));
            return this;
        }
        public MqttTargetBlockOptionsBuilder<T> WithInputOptions(DataflowBlockOptions options)
        {
            _options.InputOptions = options;
            return this;
        }
        public MqttTargetBlockOptionsBuilder<T> WithEncoderOptions(ExecutionDataflowBlockOptions options)
        {
            _options.EncoderOptions = options;
            return this;
        }
        public MqttTargetBlockOptionsBuilder<T> WithPublishOptions(ExecutionDataflowBlockOptions options)
        {
            _options.PublishOptions = options;
            return this;
        }
        public MqttTargetBlockOptionsBuilder<T> WithPublishRetryMaxDelays(TimeSpan delay)
        {
            _options.PublishRetryMaxDelay = delay;
            return this;
        }
        public MqttTargetBlockOptionsBuilder<T> WithCodec(IMqttTargetBlockCodec<T> c)
        {
            _options.Codec = c;
            return this;
        }
        public MqttTargetBlockOptions<T> Build() => _options;
    }
}
