using MQTTnet;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace BlueForest.MqttNet.Dataflow
{
    public class MqttSourceBlockOptions<T> : IMqttSourceBlockOptions<T>
    {
        bool _ft = true;
        List<MqttTopicFilter> _filters = new List<MqttTopicFilter>(1);

        public IManagedMqttClient ManagedClient { get; set; }
        public List<MqttTopicFilter> Topics { get => _filters; set => _filters = value; }
        public bool FilteringTopics { get => _ft; set => _ft = value; }
        public DataflowBlockOptions SourceOptions { get; set; }
        public ExecutionDataflowBlockOptions DecoderOptions { get; set; }
        public DataflowBlockOptions TargetOptions { get; set; }
        public IMqttSourceBlockCodec<T> Codec { get; set; }
    }
}
