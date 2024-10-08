using DevStore.Core.Data;
using System.Threading.Tasks;

namespace DevStore.Orders.Domain.Vouchers
{
    public interface IVoucherRepository : IRepository<Voucher>
    {
        Task<Voucher> GetVoucherByCode(string code);
        void Update(Voucher voucher);
    }
}