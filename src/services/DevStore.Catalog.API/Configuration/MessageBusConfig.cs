using DevStore.Catalog.API.Services;
using DevStore.Core.Utils;
using DevStore.MessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevStore.Catalog.API.Configuration
{
    public static class MessageBusConfig
    {
        public static void AddMessageBusConfiguration(this IServiceCollection services, string connection)
        {
            services.AddMessageBus(connection)
                .AddHostedService<CatalogIntegrationHandler>();
        }
    }
}