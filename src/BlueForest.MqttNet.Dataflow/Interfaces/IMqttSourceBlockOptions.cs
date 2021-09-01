using MQTTnet;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace BlueForest.MqttNet.Dataflow
{
    public interface IMqttSourceBlockOptions<T>
    {
        IManagedMqttClient ManagedClient { get;}
        ExecutionDataflowBlockOptions DecoderOptions { get; }
        bool FilteringTopics { get; }
        DataflowBlockOptions SourceOptions { get; }
        DataflowBlockOptions TargetOptions { get; }
        List<MqttTopicFilter> Topics { get; }
        IMqttSourceBlockCodec<T> Codec { get; }
    }
}