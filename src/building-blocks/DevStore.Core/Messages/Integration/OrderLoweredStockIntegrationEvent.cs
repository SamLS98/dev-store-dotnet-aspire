using System;

namespace DevStore.Core.Messages.Integration
{
    public class OrderLoweredStockIntegrationEvent(Guid customerId, Guid orderId) : IntegrationEvent
    {
        public Guid CustomerId { get; private set; } = customerId;
        public Guid OrderId { get; private set; } = orderId;
    }
}