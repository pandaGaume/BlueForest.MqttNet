using MQTTnet;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace BlueForest.MqttNet.Dataflow
{
    public class MqttSourceOptions
    {
        bool _ft = true;
        List<MqttTopicFilter> _filters = new List<MqttTopicFilter>(1);
        IManagedMqttClient _c = null;

        public IManagedMqttClient MqttClient { get => _c; set => _c = value; }

        public List<MqttTopicFilter> Topics { get => _filters; set => _filters = value; }

        public bool FilteringTopics { get => _ft; set => _ft = value; }

        public DataflowBlockOptions SourceOptions { get; set; }
        public ExecutionDataflowBlockOptions EncoderOptions { get; set; }
        public DataflowBlockOptions TargetOptions { get; set; }
    }
}
