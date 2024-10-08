using DevStore.Billing.API.Facade;
using DevStore.Billing.API.Models;
using DevStore.Core.DomainObjects;
using DevStore.Core.Messages.Integration;
using FluentValidation.Results;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DevStore.Billing.API.Services
{
    public class BillingService(IPaymentFacade paymentFacade,
                          IPaymentRepository paymentRepository) : IBillingService
    {
        public async Task<ResponseMessage> AuthorizeTransaction(Payment payment)
        {
            var transaction = await paymentFacade.AuthorizePayment(payment);
            var validationResult = new ValidationResult();

            if (transaction.TransactionStatus != TransactionStatus.Authorized)
            {
                validationResult.Errors.Add(new ValidationFailure("Payment",
                        "Payment refused, please contact your card operator"));

                return new ResponseMessage(validationResult);
            }

            payment.AdicionarTransacao(transaction);
            paymentRepository.AddPayment(payment);

            if (!await paymentRepository.UnitOfWork.Commit())
            {
                validationResult.Errors.Add(new ValidationFailure("Payment",
                    "There was an error while making the payment."));

                // Canceling the payment on the service
                await CancelTransaction(payment.OrderId);

                return new ResponseMessage(validationResult);
            }

            return new ResponseMessage(validationResult);
        }

        public async Task<ResponseMessage> GetTransaction(Guid orderId)
        {
            var transactions = await paymentRepository.GetTransactionsByOrderId(orderId);
            var authorizedTransaction = transactions?.FirstOrDefault(t => t.TransactionStatus == TransactionStatus.Authorized);
            var validationResult = new ValidationResult();

            if (authorizedTransaction == null) throw new DomainException($"Transaction not found for order {orderId}");

            var transaction = await paymentFacade.CapturePayment(authorizedTransaction);

            if (transaction.TransactionStatus != TransactionStatus.Paid)
            {
                validationResult.Errors.Add(new ValidationFailure("Payment",
                    $"Unable to capture order payment {orderId}"));

                return new ResponseMessage(validationResult);
            }

            transaction.PaymentId = authorizedTransaction.PaymentId;
            paymentRepository.AddTransaction(transaction);

            if (!await paymentRepository.UnitOfWork.Commit())
            {
                validationResult.Errors.Add(new ValidationFailure("Payment",
                    $"It was not possible to persist the capture of the payment of the order {orderId}"));

                return new ResponseMessage(validationResult);
            }

            return new ResponseMessage(validationResult);
        }

        public async Task<ResponseMessage> CancelTransaction(Guid orderId)
        {
            var transactions = await paymentRepository.GetTransactionsByOrderId(orderId);
            var authorizedTransaction = transactions?.FirstOrDefault(t => t.TransactionStatus == TransactionStatus.Authorized);
            var validationResult = new ValidationResult();

            if (authorizedTransaction == null) throw new DomainException($"Transaction not found for order {orderId}");

            var transaction = await paymentFacade.CancelAuthorization(authorizedTransaction);

            if (transaction.TransactionStatus != TransactionStatus.Canceled)
            {
                validationResult.Errors.Add(new ValidationFailure("Payment",
                    $"Unable to cancel order payment {orderId}"));

                return new ResponseMessage(validationResult);
            }

            transaction.PaymentId = authorizedTransaction.PaymentId;
            paymentRepository.AddTransaction(transaction);

            if (!await paymentRepository.UnitOfWork.Commit())
            {
                validationResult.Errors.Add(new ValidationFailure("Payment",
                    $"It was not possible to persist the cancellation of the order payment {orderId}"));

                return new ResponseMessage(validationResult);
            }

            return new ResponseMessage(validationResult);
        }
    }
}