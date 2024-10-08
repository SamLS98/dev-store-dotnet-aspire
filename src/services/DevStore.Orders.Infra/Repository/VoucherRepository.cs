using DevStore.Core.Data;
using DevStore.Orders.Domain.Vouchers;
using DevStore.Orders.Infra.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DevStore.Orders.Infra.Repository
{
    public class VoucherRepository(OrdersContext context) : IVoucherRepository
    {
        public IUnitOfWork UnitOfWork => context;

        public async Task<Voucher> GetVoucherByCode(string code)
        {
            return await context.Vouchers.FirstOrDefaultAsync(p => p.Code == code);
        }

        public void Update(Voucher voucher)
        {
            context.Vouchers.Update(voucher);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            context.Dispose();
        }
    }
}