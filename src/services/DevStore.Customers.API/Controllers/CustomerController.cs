using DevStore.Customers.API.Application.Commands;
using DevStore.Customers.API.Models;
using DevStore.WebAPI.Core.Controllers;
using DevStore.WebAPI.Core.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DevStore.Customers.API.Controllers
{
    [Route("customers")]
    public class CustomerController(ICustomerRepository customerRepository, IMediator mediator, IAspNetUser user) : MainController
    {
        [HttpGet("address")]
        public async Task<IActionResult> GetAddress()
        {
            var address = await customerRepository.GetAddressById(user.GetUserId());

            return address == null ? NotFound() : CustomResponse(address);
        }

        [HttpPost("address")]
        public async Task<IActionResult> AddAddress(AddAddressCommand address)
        {
            address.CustomerId = user.GetUserId();
            return CustomResponse(await mediator.Send(address));
        }
    }
}