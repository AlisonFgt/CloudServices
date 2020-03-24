using Microsoft.Extensions.Configuration;
using System.IO;

namespace CloudServices.Common
{
    public static class AppSettingsHelper
    {
        public static IConfigurationRoot Builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        public static string GetCloud()
        {
            return Builder.GetSection("Cloud").Value;
        }

        public static string GetConfig(string param)
        {
            return Builder.GetSection(param).Value;
        }
    }
}
