using DevStore.Core.DomainObjects;
using DevStore.Orders.Domain.Vouchers.Specs;
using System;

namespace DevStore.Orders.Domain.Vouchers
{
    public class Voucher(string code, decimal? percentage, decimal? discount, int quantity, VoucherDiscountType discountType, DateTime expirationDate) : Entity, IAggregateRoot
    {
        public string Code { get; private set; } = code;
        public decimal? Percentage { get; private set; } = percentage;
        public decimal? Discount { get; private set; } = discount;
        public int Quantity { get; private set; } = quantity;
        public VoucherDiscountType DiscountType { get; private set; } = discountType;
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public DateTime? UsedAt { get; private set; }
        public DateTime ExpirationDate { get; private set; } = expirationDate;
        public bool Active { get; private set; } = true;
        public bool Used { get; private set; } = false;

        public bool CanUse()
        {
            return new VoucherActiveSpecification()
                .And(new VoucherDateSpecification())
                .And(new VoucherQuantitySpecification())
                .IsSatisfiedBy(this);
        }

        public void SetAsUsed()
        {
            Active = false;
            Used = true;
            Quantity = 0;
            UsedAt = DateTime.Now;
        }

        public void GetOne()
        {
            Quantity -= 1;
            if (Quantity >= 1) return;

            SetAsUsed();
        }
    }
}