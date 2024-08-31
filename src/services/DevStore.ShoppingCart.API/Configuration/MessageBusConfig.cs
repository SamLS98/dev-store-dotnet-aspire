using DevStore.Core.Utils;
using DevStore.MessageBus;
using DevStore.ShoppingCart.API.Services;

namespace DevStore.ShoppingCart.API.Configuration
{
    public static class MessageBusConfig
    {
        public static void AddMessageBusConfiguration(this IServiceCollection services, string connection)
        {
            services.AddMessageBus(connection)
                .AddHostedService<ShoppingCartIntegrationHandler>();
        }
    }
}