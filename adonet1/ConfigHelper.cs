using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace adonet1
{
    public static class ConfigHelper
    {
        public static IConfiguration GetConfiguration() =>
            new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    }
}
