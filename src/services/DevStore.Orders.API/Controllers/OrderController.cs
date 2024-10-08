using DevStore.Core.Mediator;
using DevStore.Orders.API.Application.Commands;
using DevStore.Orders.API.Application.DTO;
using DevStore.Orders.API.Application.Queries;
using DevStore.WebAPI.Core.Controllers;
using DevStore.WebAPI.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevStore.Orders.API.Controllers
{
    [Authorize, Route("orders")]
    public class OrderController(IMediatorHandler mediator,
        IAspNetUser user,
        IOrderQueries orderQueries) : MainController
    {
        [HttpPost("")]
        public async Task<IActionResult> AddOrder(AddOrderCommand order)
        {
            order.CustomerId = user.GetUserId();
            return CustomResponse(await mediator.SendCommand(order));
        }

        [HttpGet("last")]
        public async Task<ActionResult<OrderDTO>> LastOrder()
        {
            var order = await orderQueries.GetLastOrder(user.GetUserId());

            return order == null ? NoContent() : CustomResponse(order);
        }

        [HttpGet("customers")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> Customers()
        {
            var orders = await orderQueries.GetByCustomerId(user.GetUserId());

            return orders == null ? NoContent() : CustomResponse(orders);
        }
    }
}