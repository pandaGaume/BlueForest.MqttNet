using MQTTnet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace BlueForest.MqttNet.Dataflow
{
    public class MqttTargetBlockOptions<T> : IMqttTargetBlockOptions<T>
    {
        List<MqttTopicFilter> _filters = new List<MqttTopicFilter>(1);

        public IManagedMqttClient ManagedClient { get; set; }
        public List<MqttTopicFilter> Topics { get => _filters; set => _filters = value; }
        public DataflowBlockOptions InputOptions { get; set; }
        public ExecutionDataflowBlockOptions EncoderOptions { get; set; }
        public ExecutionDataflowBlockOptions PublishOptions { get; set; }
        public IMqttTargetBlockCodec<T> Codec { get; set; }
        public TimeSpan? PublishRetryMaxDelay { get; set; }

    }
}
