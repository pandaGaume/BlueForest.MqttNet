using MQTTnet;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace BlueForest.MqttNet.Dataflow
{
    public class MqttSourceBlockOptionsBuilder<T>
    {
        MqttSourceBlockOptions<T> _options = new MqttSourceBlockOptions<T>();

        public MqttSourceBlockOptionsBuilder<T> WithMqttClient(IManagedMqttClient c)
        {
            _options.ManagedClient = c;
            return this;
        }
        public MqttSourceBlockOptionsBuilder<T> WithMqttClient(ManagedMqttClientBuilder cb)
        {
            _options.ManagedClient = cb.Build();
            return this;
        }
        public MqttSourceBlockOptionsBuilder<T> WithTopic(MqttTopicFilter topic)
        {
            _options.Topics.Add(topic);
            return this;
        }
        public MqttSourceBlockOptionsBuilder<T> WithTopic(MqttTopicFilterBuilder topicBuilder)
        {
            _options.Topics.Add(topicBuilder.Build());
            return this;
        }
        public MqttSourceBlockOptionsBuilder<T> WithTopics(IEnumerable<MqttTopicFilter> topics)
        {
            _options.Topics.AddRange(topics);
            return this;
        }
        public MqttSourceBlockOptionsBuilder<T> WithTopics(IEnumerable<MqttTopicFilterBuilder> topicBuilders)
        {
            _options.Topics.AddRange(topicBuilders.Select(b=>b.Build()));
            return this;
        }
        public MqttSourceBlockOptionsBuilder<T> WithFilteringTopics(bool filtering)
        {
            _options.FilteringTopics = filtering;
            return this;
        }
        public MqttSourceBlockOptionsBuilder<T> WithSourceOptions(DataflowBlockOptions options)
        {
            _options.SourceOptions = options;
            return this;
        }
        public MqttSourceBlockOptionsBuilder<T> WithEncoderOptions(ExecutionDataflowBlockOptions options)
        {
            _options.DecoderOptions = options;
            return this;
        }
        public MqttSourceBlockOptionsBuilder<T> WithTargetOptions(DataflowBlockOptions options)
        {
            _options.TargetOptions = options;
            return this;
        }
        public MqttSourceBlockOptionsBuilder<T> WithCodec(IMqttSourceBlockCodec<T> c)
        {
            _options.Codec = c;
            return this;
        }
        public MqttSourceBlockOptions<T> Build() => _options;
    }
}
