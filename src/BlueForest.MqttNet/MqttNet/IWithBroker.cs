namespace BlueForest.MqttNet
{
    public interface IWithBroker
    {
        IManagedMqttClient Broker { get; }
    }
}
