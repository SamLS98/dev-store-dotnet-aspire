using DevStore.Core.Messages;
using System;

namespace DevStore.Orders.API.Application.Events
{
    public class OrderDoneEvent(Guid orderId, Guid customerId) : Event
    {
        public Guid OrderId { get; private set; } = orderId;
        public Guid CustomerId { get; private set; } = customerId;
    }
}