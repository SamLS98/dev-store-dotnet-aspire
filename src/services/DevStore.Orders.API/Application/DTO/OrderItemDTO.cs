using DevStore.Orders.Domain.Orders;
using System;

namespace DevStore.Orders.API.Application.DTO
{
    public class OrderItemDTO
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }

        public static OrderItem ToOrderItem(OrderItemDTO orderItemDto)
        {
            return new OrderItem(orderItemDto.ProductId, orderItemDto.Name, orderItemDto.Quantity,
                orderItemDto.Price, orderItemDto.Image);
        }
    }
}