using MQTTnet.Client.Options;

namespace BlueForest.MqttNet
{
    public static class BrokerOptionsExtensions
    {
        public static IMqttClientOptions BuildMqttClientOptions(this BrokerOptions settings)
        {

            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithClientId(settings.ClientId)
                .WithTcpServer(settings.Host, settings.Port)
                .WithCleanSession();

            optionsBuilder = settings.Credentials != null && settings.Credentials.Username != null && settings.Credentials.Password != null ? optionsBuilder.WithCredentials(settings.Credentials.Username, settings.Credentials.Password) : optionsBuilder;
            var options = (settings.IsSecure ?? false ? optionsBuilder.WithTls() : optionsBuilder).Build();
            return options;
        }
    }
}
