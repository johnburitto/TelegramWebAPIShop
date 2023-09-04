using Microsoft.Extensions.Configuration;
using System;

namespace Bot.Utils
{
    public class ConfigurationReader
    {
        public static T ReadSection<T>(string sectionName)
        {
            var environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables();
            var configuration = builder.Build();

            return configuration.GetSection(sectionName).Get<T>() ?? throw new Exception("Usersecret or section doesn't exist");
        }
    }
}
