using System;
using System.Collections.Generic;

namespace DevStore.Core.Messages.Integration
{
    public class OrderAuthorizedIntegrationEvent(Guid customerId, Guid orderId, IDictionary<Guid, int> items) : IntegrationEvent
    {
        public Guid CustomerId { get; private set; } = customerId;
        public Guid OrderId { get; private set; } = orderId;
        public IDictionary<Guid, int> Items { get; private set; } = items;
    }
}