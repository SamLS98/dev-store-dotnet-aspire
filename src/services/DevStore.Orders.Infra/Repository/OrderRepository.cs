using DevStore.Core.Data;
using DevStore.Orders.Domain.Orders;
using DevStore.Orders.Infra.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace DevStore.Orders.Infra.Repository
{
    public class OrderRepository(OrdersContext context) : IOrderRepository
    {
        public IUnitOfWork UnitOfWork => context;

        public DbConnection GetConnection() => context.Database.GetDbConnection();

        public async Task<Order> GetById(Guid id)
        {
            return await context.Orders.FindAsync(id);
        }

        public async Task<IEnumerable<Order>> GetCustomersById(Guid customerId)
        {
            return await context.Orders
                .Include(p => p.OrderItems)
                .AsNoTracking()
                .Where(p => p.CustomerId == customerId)
                .ToListAsync();
        }

        public void Add(Order order)
        {
            context.Orders.Add(order);
        }

        public void Update(Order order)
        {
            context.Orders.Update(order);
        }


        public async Task<OrderItem> GetItemById(Guid id)
        {
            return await context.OrderItems.FindAsync(id);
        }

        public async Task<OrderItem> GetItemByOrder(Guid orderId, Guid productId)
        {
            return await context.OrderItems
                .FirstOrDefaultAsync(p => p.ProductId == productId && p.OrderId == orderId);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            context.Dispose();
        }

        public Task<Order> GetLastOrder(Guid customerId)
        {
            var fiveMinutesAgo = DateTime.Now.AddMinutes(-5);

            return context.Orders
                .Include(i => i.OrderItems)
                .Where(o => o.CustomerId == customerId && o.DateAdded > fiveMinutesAgo && o.DateAdded <= DateTime.Now)
                .OrderByDescending(o => o.DateAdded).FirstOrDefaultAsync();
        }

        public Task<Order> GetLastAuthorizedOrder()
        {
            return context.Orders.Include(i => i.OrderItems)
                .Where(o => o.OrderStatus == OrderStatus.Authorized)
                .OrderBy(o => o.DateAdded).FirstOrDefaultAsync();


        }
    }
}