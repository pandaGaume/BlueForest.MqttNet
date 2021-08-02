using System;

namespace BlueForest.MqttNet
{
    public class ManagedBrokerOptionsBuilder
    {
        ManagedBrokerOptions _options = new ManagedBrokerOptions();

        public ManagedBrokerOptionsBuilder WithMqttBrokerSettings(BrokerOptions s)
        {
            _options.ClientOptions = s;
            return this;
        }

        public ManagedBrokerOptionsBuilder WithAutoReconnectMaxDelay(TimeSpan timeSpan)
        {
            _options.AutoReconnectMaxDelay = timeSpan;
            return this;
        }

        public ManagedBrokerOptions Build() => _options;
    }
}
