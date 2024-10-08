using DevStore.Core.Utils;
using DevStore.MessageBus;
using DevStore.Orders.API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevStore.Orders.API.Configuration
{
    public static class MessageBusConfig
    {
        public static void AddMessageBusConfiguration(this IServiceCollection services, string connection)
        {
            services.AddMessageBus(connection)
                .AddHostedService<OrderOrquestratorIntegrationHandler>()
                .AddHostedService<OrderIntegrationHandler>();
        }
    }
}