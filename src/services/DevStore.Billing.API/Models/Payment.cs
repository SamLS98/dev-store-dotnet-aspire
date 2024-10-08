using DevStore.Core.DomainObjects;
using System;
using System.Collections.Generic;

namespace DevStore.Billing.API.Models
{
    public class Payment : Entity, IAggregateRoot
    {
        public Payment()
        {
            Transactions = [];
        }

        public Guid OrderId { get; set; }
        public PaymentType PaymentType { get; set; }
        public decimal Amount { get; set; }

        public CreditCard CreditCard { get; set; }

        // EF Relation
        public ICollection<Transaction> Transactions { get; set; }

        public void AdicionarTransacao(Transaction transaction)
        {
            Transactions.Add(transaction);
        }
    }
}