using MQTTnet.Client.Options;
using System.Security.Authentication;

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

            optionsBuilder = (settings?.Credentials.Username != null && settings?.Credentials.Password != null) ? optionsBuilder.WithCredentials(settings.Credentials.Username, settings.Credentials.Password) : optionsBuilder;
            if (settings.IsSecure ?? false)
            {
                MqttClientOptionsBuilderTlsParameters tls = new MqttClientOptionsBuilderTlsParameters()
                {
                    UseTls = true,
                    SslProtocol = settings.Tls?.SslProtocol ?? TlsOptions.DefaultProtocol
                };
                optionsBuilder = optionsBuilder.WithTls(tls) ;
            }
            var options = optionsBuilder.Build();

            return options;
        }
    }
}
