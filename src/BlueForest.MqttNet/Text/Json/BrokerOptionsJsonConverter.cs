using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlueForest.MqttNet.Text.Json
{
    public class BrokerOptionsJsonConverter : JsonConverter<BrokerOptions>
    {
        public override BrokerOptions Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                // load from uri.
                var path = reader.GetString();
                if (Uri.TryCreate(path, UriKind.Absolute, out Uri uri))
                {
                    using (WebClient client = new WebClient())
                    {
                        using (var stream = client.OpenRead(uri))
                        {
                            var utf8 = new StreamReader(stream, Encoding.UTF8).ReadToEnd();
                            return JsonSerializer.Deserialize<BrokerOptions>(utf8, options);
                        }
                    }
                }
                // load from local file
                FileInfo info = new FileInfo(path);
                if (info.Exists)
                {
                    using (var stream = info.OpenRead())
                    {
                        var utf8 = new StreamReader(stream, Encoding.UTF8).ReadToEnd();
                        return JsonSerializer.Deserialize<BrokerOptions>(utf8, options);
                    }
                }
                return null;

            }
            // read online
            return JsonSerializer.Deserialize<BrokerOptions>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, BrokerOptions value, JsonSerializerOptions options) => JsonSerializer.Serialize<BrokerOptions>(writer, value, options);
       
    }
}
