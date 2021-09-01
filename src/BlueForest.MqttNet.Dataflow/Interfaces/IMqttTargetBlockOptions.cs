using MQTTnet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace BlueForest.MqttNet.Dataflow
{
    public interface IMqttTargetBlockOptions<T>
    {
        IManagedMqttClient ManagedClient { get; }
        List<MqttTopicFilter> Topics { get; }
        IMqttTargetBlockCodec<T> Codec { get; }
        public DataflowBlockOptions InputOptions { get; }
        public ExecutionDataflowBlockOptions EncoderOptions { get; }
        public ExecutionDataflowBlockOptions PublishOptions { get; }
        public TimeSpan? PublishRetryMaxDelay { get; }

    }
}
