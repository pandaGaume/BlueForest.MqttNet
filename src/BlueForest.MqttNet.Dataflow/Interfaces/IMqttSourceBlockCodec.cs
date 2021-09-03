using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlueForest.MqttNet.Dataflow
{
    public interface IMqttSourceBlockCodec<T>
    {
        ValueTask<IEnumerable<T>> GetMessageAsync(byte[] payload, CancellationToken cancel = default);
    }
}
