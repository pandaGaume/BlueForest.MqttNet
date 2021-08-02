using System;

namespace BlueForest.MqttNet
{
    public class ManagedBrokerOptions
    {
        public BrokerOptions ClientOptions { get; set; }
        public TimeSpan? AutoReconnectMaxDelay { get; set; }
    }
}