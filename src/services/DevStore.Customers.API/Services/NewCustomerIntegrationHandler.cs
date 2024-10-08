using DevStore.Core.Messages.Integration;
using DevStore.Customers.API.Application.Commands;
using DevStore.MessageBus;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevStore.Customers.API.Services
{
    public class NewCustomerIntegrationHandler(
                        IServiceProvider serviceProvider,
                        IMessageBus bus) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await bus.RespondAsync<UserRegisteredIntegrationEvent, ResponseMessage>(AddCustomer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }

        private async Task<ResponseMessage> AddCustomer(UserRegisteredIntegrationEvent message)
        {
            var customerCommand = new NewCustomerCommand(message.Id, message.Name, message.Email, message.SocialNumber);
            ValidationResult sucesso;

            using (var scope = serviceProvider.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                sucesso = await mediator.Send(customerCommand);
            }

            return new ResponseMessage(sucesso);
        }

    }
}