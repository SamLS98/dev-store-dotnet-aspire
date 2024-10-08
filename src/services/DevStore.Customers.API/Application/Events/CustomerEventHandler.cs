using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Console = System.Console;

namespace DevStore.Customers.API.Application.Events
{
    public class CustomerEventHandler : INotificationHandler<NewCustomerAddedEvent>
    {
        public Task Handle(NewCustomerAddedEvent notification, CancellationToken cancellationToken)
        {
            // Send confirmation event
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("*****************************************************************");
            Console.WriteLine($"The aggregate event {notification.AggregateId} was manipulated!");
            Console.WriteLine("*****************************************************************");
            Console.ForegroundColor = ConsoleColor.White;

            return Task.CompletedTask;
        }
    }
}