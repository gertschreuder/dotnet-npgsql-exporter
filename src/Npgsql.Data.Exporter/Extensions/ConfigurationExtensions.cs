using System.IO;
using Microsoft.Extensions.Configuration;
using Npgsql.Data.Exporter.Models;

namespace Npgsql.Data.Exporter.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IConfiguration Get(string path = "appsettings.json")
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path, optional: false)
                .Build();

            return config;
        }

        public static NpgsqlDataExporterConfiguration GetNpgsqlDataExporter(this IConfiguration configuration)
        {
            var npgsqlDataExporterConfiguration = new NpgsqlDataExporterConfiguration();
            configuration.GetSection("NpgsqlDataExporterConfiguration").Bind(npgsqlDataExporterConfiguration);

            return npgsqlDataExporterConfiguration;
        }

        public static NpgsqlDataExporterConfiguration GetNpgsqlDataExporter(string path = "appsettings.json")
        {
            var configuration = Get(path);

            return GetNpgsqlDataExporter(configuration);
        }
    }
}