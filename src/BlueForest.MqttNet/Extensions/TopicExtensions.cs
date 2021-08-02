namespace BlueForest.MqttNet
{
    public static class TopicExtensions
    {
        public static bool Match(this string a, string b) => a.Split(MqttProtocol.TOPIC_SEPARATOR).Match(b.Split(MqttProtocol.TOPIC_SEPARATOR));
        public static bool Match(this string[] a, string[] b)
        {
            if (a.Length > b.Length)
            {
                return false;
            }

            for (var i = 0; i != a.Length; i++)
            {
                var p0 = a[i];
                if (p0 == MqttProtocol.SINGLE_LEVEL_WILD_STR)
                {
                    continue;
                }
                if (p0 == MqttProtocol.MULTI_LEVEL_WILD_STR)
                {
                    return true;
                }
                if (p0.CompareTo(b[i]) != 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
