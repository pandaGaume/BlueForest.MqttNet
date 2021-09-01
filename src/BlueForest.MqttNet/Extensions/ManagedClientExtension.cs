using MQTTnet;
using MQTTnet.Client.Publishing;
using System.Threading;
using System.Threading.Tasks;

namespace BlueForest.MqttNet
{
    public static class ManagedClientExtension
    {
        public static bool IsConnected(this IManagedMqttClient c) => (c?.Client?.IsConnected) ?? false;
        public static Task<MqttClientPublishResult> PublishAsync(this IManagedMqttClient c, MqttApplicationMessage applicationMessage, CancellationToken cancellationToken) => c.Client.PublishAsync(applicationMessage, cancellationToken);
    }
}
