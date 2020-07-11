using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql.Data.Exporter.Models;

namespace Npgsql.Data.Exporter.Extensions
{
    public static class StartupExtensions
    {
        public static TSettings AddConfig<TSettings>(this IServiceCollection services, IConfiguration configuration)
            where TSettings : class, new()
        {
            return services.AddConfig<TSettings>(configuration, options => { });
        }

        public static TSettings AddConfig<TSettings>(this IServiceCollection services, IConfiguration configuration,
            Action<BinderOptions> configureOptions)
            where TSettings : class, new()
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            TSettings setting = configuration.Get<TSettings>(configureOptions);
            services.TryAddSingleton(setting);
            return setting;
        }

        public static IServiceCollection AddNpgsqlDataExporter(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddConfig<NpgsqlDataExporterConfiguration>(
                configuration.GetSection(nameof(NpgsqlDataExporterConfiguration)));

            return services;
        }

        
    }
}