using System.Threading.Tasks;

namespace BlueForest.MqttNet.Dataflow
{
    public interface IMqttTargetBlockCodec<T>
    {
        ValueTask<byte[]> GetPayloadAsync(T data);
        string GetTopic(string topic, T data);
    }
}
