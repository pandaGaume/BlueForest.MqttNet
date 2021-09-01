using System;
using System.Text;

namespace BlueForest.MqttNet
{
    public class BrokerOptionsBuilder
    {
        BrokerOptions _options = new BrokerOptions();

        public BrokerOptionsBuilder WithHost(string host)
        {
            _options.Host = host;
            return this;
        }
        public BrokerOptionsBuilder WithPort(int? port)
        {
            _options.Port = port;
            return this;
        }
        public BrokerOptionsBuilder WithSecure(bool secure)
        {
            _options.IsSecure = secure;
            return this;
        }

        public BrokerOptionsBuilder WithCredentials(string username, string password = null) => WithCredentials(new BrokerClientCredential
        {
            Username = username,
            Password = password
        });

        public BrokerOptionsBuilder WithCredentials(string username, byte[] password = null)=> WithCredentials(new BrokerClientCredential
        {
            Username = username,
            Password = Encoding.UTF8.GetString(password)
        });
        

        public BrokerOptionsBuilder WithCredentials(BrokerClientCredential credentials)
        {
            _options.Credentials = credentials;
            return this;
        }
        public BrokerOptions Build() => _options;
    }
}
