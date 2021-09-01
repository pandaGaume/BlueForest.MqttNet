using System.Security.Authentication;
using System.Text.Json.Serialization;

namespace BlueForest.MqttNet
{
    public class TlsOptions
    {
        public static SslProtocols DefaultProtocol = SslProtocols.Tls11;

        SslProtocols? _p;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SslProtocols? SslProtocol { get => _p ?? DefaultProtocol; set => _p = value; }

    }
}
