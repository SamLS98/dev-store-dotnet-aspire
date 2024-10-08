using DevStore.Orders.API.Application.DTO;
using DevStore.Orders.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevStore.Orders.API.Application.Queries
{
    public interface IOrderQueries
    {
        Task<OrderDTO> GetLastOrder(Guid customerId);
        Task<IEnumerable<OrderDTO>> GetByCustomerId(Guid customerId);
        Task<OrderDTO> GetAuthorizedOrders();
    }

    public class OrderQueries(IOrderRepository orderRepository) : IOrderQueries
    {
        public async Task<OrderDTO> GetLastOrder(Guid customerId)
        {
            var order = await orderRepository.GetLastOrder(customerId);

            if (order is null)
                return null;

            return MapOrder(order);
        }

        public async Task<IEnumerable<OrderDTO>> GetByCustomerId(Guid customerId)
        {
            var orders = await orderRepository.GetCustomersById(customerId);

            return orders.Select(OrderDTO.ToOrderDTO);
        }

        public async Task<OrderDTO> GetAuthorizedOrders()
        {
            var orders = await orderRepository.GetLastAuthorizedOrder();

            return MapOrder(orders);
        }

        private static OrderDTO MapOrder(Order order)
        {
            if (order is null)
                return null;

            var orderDto = new OrderDTO
            {
                Id = order.Id,
                Code = order.Code,
                CustomerId = order.CustomerId,
                Status = (int)order.OrderStatus,
                Date = order.DateAdded,
                Amount = order.Amount,
                Discount = order.Discount,
                HasVoucher = order.HasVoucher,
                Address = new AddressDto
                {
                    StreetAddress = order.Address.StreetAddress,
                    Neighborhood = order.Address.Neighborhood,
                    ZipCode = order.Address.ZipCode,
                    City = order.Address.City,
                    SecondaryAddress = order.Address.SecondaryAddress,
                    State = order.Address.State,
                    BuildingNumber = order.Address.BuildingNumber
                },
                OrderItems = order.OrderItems.Select(item => new OrderItemDTO
                {
                    OrderId = item.OrderId,
                    ProductId = item.ProductId,
                    Name = item.ProductName,
                    Price = item.Price,
                    Image = item.ProductImage,
                    Quantity = item.Quantity
                }).ToList()
            };

            return orderDto;
        }

    }

}