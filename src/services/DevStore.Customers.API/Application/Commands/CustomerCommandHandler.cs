using DevStore.Core.Messages;
using DevStore.Customers.API.Application.Events;
using DevStore.Customers.API.Models;
using FluentValidation.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DevStore.Customers.API.Application.Commands
{
    public class CustomerCommandHandler(ICustomerRepository customerRepository) : CommandHandler,
        IRequestHandler<NewCustomerCommand, ValidationResult>,
        IRequestHandler<AddAddressCommand, ValidationResult>
    {
        public async Task<ValidationResult> Handle(NewCustomerCommand message, CancellationToken cancellationToken)
        {
            if (!message.IsValid()) return message.ValidationResult;

            var customer = new Customer(message.Id, message.Name, message.Email, message.SocialNumber);

            var customerExist = await customerRepository.GetBySocialNumber(customer.SocialNumber);

            if (customerExist != null)
            {
                AddError("Already has this social number.");
                return ValidationResult;
            }

            customerRepository.Add(customer);

            customer.AddEvent(new NewCustomerAddedEvent(message.Id, message.Name, message.Email, message.SocialNumber));

            return await PersistData(customerRepository.UnitOfWork);
        }

        public async Task<ValidationResult> Handle(AddAddressCommand message, CancellationToken cancellationToken)
        {
            if (!message.IsValid()) return message.ValidationResult;

            var endereco = new Address(message.StreetAddress, message.BuildingNumber, message.SecondaryAddress, message.Neighborhood, message.ZipCode, message.City, message.State, message.CustomerId);
            customerRepository.AddAddress(endereco);

            return await PersistData(customerRepository.UnitOfWork);
        }
    }
}