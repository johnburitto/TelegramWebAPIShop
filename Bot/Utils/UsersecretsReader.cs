using Microsoft.Extensions.Configuration;

namespace Bot.Utils
{
    public class UsersecretsReader
    {
        public static T ReadSection<T>(string sectionName)
        {
            var environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables();
            var configuration = builder.Build();

            return configuration.GetSection(sectionName).Get<T>() ?? throw new Exception("Usersecret or section doesn't exist");
        }
    }
}
