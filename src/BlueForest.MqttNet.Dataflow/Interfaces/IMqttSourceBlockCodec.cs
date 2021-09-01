using System.Threading;
using System.Threading.Tasks;

namespace BlueForest.MqttNet.Dataflow
{
    public interface IMqttSourceBlockCodec<T>
    {
        ValueTask<T> GetMessageAsync(byte[] payload, CancellationToken cancel = default);
    }
}
