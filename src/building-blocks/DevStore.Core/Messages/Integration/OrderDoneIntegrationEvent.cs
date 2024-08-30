using System;

namespace DevStore.Core.Messages.Integration
{
    public class OrderDoneIntegrationEvent(Guid customerId) : IntegrationEvent
    {
        public Guid CustomerId { get; private set; } = customerId;
    }
}