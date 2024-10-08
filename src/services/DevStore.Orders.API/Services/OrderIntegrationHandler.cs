using DevStore.Core.DomainObjects;
using DevStore.Core.Messages.Integration;
using DevStore.MessageBus;
using DevStore.Orders.Domain.Orders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevStore.Orders.API.Services
{
    public class OrderIntegrationHandler(IServiceProvider serviceProvider, IMessageBus bus) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await bus.SubscribeAsync<OrderCanceledIntegrationEvent>("OrderCanceled", CancelOrder);

            await bus.SubscribeAsync<OrderPaidIntegrationEvent>("OrderPaid", FinishOrder);
        }

        private async Task CancelOrder(OrderCanceledIntegrationEvent message)
        {
            using var scope = serviceProvider.CreateScope();

            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

            var order = await orderRepository.GetById(message.OrderId);
            order.Cancel();

            orderRepository.Update(order);

            if (!await orderRepository.UnitOfWork.Commit())
            {
                throw new DomainException($"Problems while trying to cancel order {message.OrderId}");
            }
        }

        private async Task FinishOrder(OrderPaidIntegrationEvent message)
        {
            using var scope = serviceProvider.CreateScope();

            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

            var order = await orderRepository.GetById(message.OrderId);
            order.Finish();

            orderRepository.Update(order);

            if (!await orderRepository.UnitOfWork.Commit())
            {
                throw new DomainException($"Problems found trying to finish o order {message.OrderId}");
            }
        }
    }
}