using DevStore.Billing.API.Services;
using DevStore.Core.Utils;
using DevStore.MessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevStore.Billing.API.Configuration
{
    public static class MessageBusConfig
    {
        public static void AddMessageBusConfiguration(this IServiceCollection services, string connection)
        {
            services.AddMessageBus(connection)
                .AddHostedService<BillingIntegrationHandler>();
        }
    }
}