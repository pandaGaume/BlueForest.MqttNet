using BlueForest.MqttNet.Text.Json;
using System;
using System.Text.Json.Serialization;

namespace BlueForest.MqttNet
{
    public class ManagedBrokerOptions
    {
        [JsonConverter(typeof(BrokerOptionsJsonConverter))]
        public BrokerOptions ClientOptions { get; set; }
        public TimeSpan? AutoReconnectMaxDelay { get; set; }
    }
}