﻿using DevStore.Core.Messages.Integration;
using DevStore.MessageBus;
using DevStore.ShoppingCart.API.Data;
using Microsoft.EntityFrameworkCore;

namespace DevStore.ShoppingCart.API.Services
{
    public class ShoppingCartIntegrationHandler(IServiceProvider serviceProvider, IMessageBus bus) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await bus.SubscribeAsync<OrderDoneIntegrationEvent>("OrderDone", RemoveShoppingCart);
        }

        private async Task RemoveShoppingCart(OrderDoneIntegrationEvent message)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ShoppingCartContext>();

            var shoppingCart = await context.CustomerShoppingCart
                .FirstOrDefaultAsync(c => c.CustomerId == message.CustomerId);

            if (shoppingCart != null)
            {
                context.CustomerShoppingCart.Remove(shoppingCart);
                await context.SaveChangesAsync();
            }
        }
    }
}