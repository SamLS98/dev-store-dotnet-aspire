using DevStore.Core.Messages.Integration;
using DevStore.MessageBus;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DevStore.Orders.API.Application.Events
{
    public class OrderEventHandler(IMessageBus bus) : INotificationHandler<OrderDoneEvent>
    {
        public async Task Handle(OrderDoneEvent message, CancellationToken cancellationToken)
        {
            await bus.PublishAsync(new OrderDoneIntegrationEvent(message.CustomerId));
        }
    }
}