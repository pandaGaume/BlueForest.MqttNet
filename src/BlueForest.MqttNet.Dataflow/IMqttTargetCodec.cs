using MQTTnet;
using System.Threading.Tasks;

namespace BlueForest.MqttNet.Dataflow
{
    public interface IMqttTargetCodec<T>
    {
        ValueTask<byte[]> GetPayloadAsync(T data);
    }
}
