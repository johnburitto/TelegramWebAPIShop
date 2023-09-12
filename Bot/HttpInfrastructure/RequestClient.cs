using Bot.Utils;

namespace Bot.HttpInfrastructure
{
    public static class RequestClient
    {
        private static HttpClient? _client;
        private static object _lock = new object();

        public static HttpClient Client => GetClient();
        public static Uri BaseAddress => new Uri(ConfigurationReader.ReadSection<string>("APIAddress"));
    
        private static HttpClient GetClient()
        {
            if (_client == null)
            {
                lock (_lock)
                {
                    _client = new HttpClient();
                    _client.BaseAddress = BaseAddress;
                }
            }

            return _client;
        }
    }
}
