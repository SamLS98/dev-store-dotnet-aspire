using DevStore.Billing.API.Models;
using DevStore.Billing.DevsPay;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Transaction = DevStore.Billing.API.Models.Transaction;
using TransactionStatus = DevStore.Billing.API.Models.TransactionStatus;

namespace DevStore.Billing.API.Facade
{
    public class CreditCardPaymentFacade(IOptions<BillingConfig> pagamentoConfig) : IPaymentFacade
    {
        private readonly BillingConfig _billingConfig = pagamentoConfig.Value;

        public async Task<Transaction> AuthorizePayment(Payment payment)
        {
            var nerdsPagSvc = new DevsPayService(_billingConfig.DefaultApiKey,
                _billingConfig.DefaultEncryptionKey);

            var cardHashGen = new CardHash(nerdsPagSvc)
            {
                CardNumber = payment.CreditCard.CardNumber,
                CardHolderName = payment.CreditCard.Holder,
                CardExpirationDate = payment.CreditCard.ExpirationDate,
                CardCvv = payment.CreditCard.SecurityCode
            };
            var cardHash = cardHashGen.Generate();

            var transacao = new DevsPay.Transaction(nerdsPagSvc)
            {
                CardHash = cardHash,
                CardNumber = payment.CreditCard.CardNumber,
                CardHolderName = payment.CreditCard.Holder,
                CardExpirationDate = payment.CreditCard.ExpirationDate,
                CardCvv = payment.CreditCard.SecurityCode,
                PaymentMethod = PaymentMethod.CreditCard,
                Amount = payment.Amount
            };

            return ToTransaction(await transacao.AuthorizeCardTransaction());
        }

        public async Task<Transaction> CapturePayment(Transaction transaction)
        {
            var nerdsPagSvc = new DevsPayService(_billingConfig.DefaultApiKey,
                _billingConfig.DefaultEncryptionKey);

            var tr = ParaTransaction(transaction, nerdsPagSvc);

            return ToTransaction(await tr.CaptureCardTransaction());
        }

        public async Task<Transaction> CancelAuthorization(Transaction transaction)
        {
            var nerdsPagSvc = new DevsPayService(_billingConfig.DefaultApiKey,
                _billingConfig.DefaultEncryptionKey);

            var tr = ParaTransaction(transaction, nerdsPagSvc);

            return ToTransaction(await tr.CancelAuthorization());
        }

        public static Transaction ToTransaction(DevsPay.Transaction transaction)
        {
            return new Transaction
            {
                Id = Guid.NewGuid(),
                TransactionStatus = (TransactionStatus)transaction.Status,
                Amount = transaction.Amount,
                CreditCardCompany = transaction.CardBrand,
                AuthorizationCode = transaction.AuthorizationCode,
                TransactionCost = transaction.Cost,
                TransactionDate = transaction.TransactionDate,
                NSU = transaction.Nsu,
                TID = transaction.Tid
            };
        }

        public static DevsPay.Transaction ParaTransaction(Transaction transaction, DevsPayService devsPayService)
        {
            return new DevsPay.Transaction(devsPayService)
            {
                Status = (DevsPay.TransactionStatus)transaction.TransactionStatus,
                Amount = transaction.Amount,
                CardBrand = transaction.CreditCardCompany,
                AuthorizationCode = transaction.AuthorizationCode,
                Cost = transaction.TransactionCost,
                Nsu = transaction.NSU,
                Tid = transaction.TID
            };
        }
    }
}