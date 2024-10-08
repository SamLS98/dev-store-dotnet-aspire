using DevStore.Billing.API.Models;
using DevStore.Core.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevStore.Billing.API.Data.Repository
{
    public class PaymentRepository(BillingContext context) : IPaymentRepository
    {
        public IUnitOfWork UnitOfWork => context;

        public void AddPayment(Payment payment)
        {
            context.Payments.Add(payment);
        }

        public void AddTransaction(Transaction transaction)
        {
            context.Transactions.Add(transaction);
        }

        public async Task<Payment> GetPaymentByOrderId(Guid orderId)
        {
            return await context.Payments.AsNoTracking()
                .FirstOrDefaultAsync(p => p.OrderId == orderId);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByOrderId(Guid orderId)
        {
            return await context.Transactions.AsNoTracking()
                .Where(t => t.Payment.OrderId == orderId).ToListAsync();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            context.Dispose();
        }
    }
}