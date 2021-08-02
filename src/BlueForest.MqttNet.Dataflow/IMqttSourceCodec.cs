using System.Threading.Tasks;

namespace BlueForest.MqttNet.Dataflow
{
    public interface IMqttSourceCodec<T>
    {
        ValueTask<T> GetMessageAsync(byte[] payload);
    }
}
