using MQTTnet;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace BlueForest.MqttNet.Dataflow
{
    public class MqttSourceOptionsBuilder
    {
        MqttSourceOptions _options = new MqttSourceOptions();

        public MqttSourceOptionsBuilder WithMqttClient(IManagedMqttClient c)
        {
            _options.MqttClient = c;
            return this;
        }
        public MqttSourceOptionsBuilder WithMqttClient(ManagedMqttClientBuilder cb)
        {
            _options.MqttClient = cb.Build();
            return this;
        }
        public MqttSourceOptionsBuilder WithTopic(MqttTopicFilter topic)
        {
            _options.Topics.Add(topic);
            return this;
        }
        public MqttSourceOptionsBuilder WithTopic(MqttTopicFilterBuilder topicBuilder)
        {
            _options.Topics.Add(topicBuilder.Build());
            return this;
        }
        public MqttSourceOptionsBuilder WithTopics(IEnumerable<MqttTopicFilter> topics)
        {
            _options.Topics.AddRange(topics);
            return this;
        }
        public MqttSourceOptionsBuilder WithTopics(IEnumerable<MqttTopicFilterBuilder> topicBuilders)
        {
            _options.Topics.AddRange(topicBuilders.Select(b=>b.Build()));
            return this;
        }

        public MqttSourceOptionsBuilder WithFilteringTopics(bool filtering)
        {
            _options.FilteringTopics = filtering;
            return this;
        }

        public MqttSourceOptionsBuilder WithSourceOptions(DataflowBlockOptions options)
        {
            _options.SourceOptions = options;
            return this;
        }
        public MqttSourceOptionsBuilder WithEncoderOptions(ExecutionDataflowBlockOptions options)
        {
            _options.EncoderOptions = options;
            return this;
        }
        public MqttSourceOptionsBuilder WithTargetOptions(DataflowBlockOptions options)
        {
            _options.TargetOptions = options;
            return this;
        }

        public MqttSourceOptions Build() => _options;
    }
}
