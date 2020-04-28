using External.PaymentGateway.Entities;
using External.PaymentGateway.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace External.PaymentGateway
{
    public class PaymentService : IPaymentService
    {
        private readonly INotificationService notificationService;
        private readonly IQueryRepository<Account> accountQuery;
        private readonly IPersistenceRepository<PaymentOrder> paymentPersistence;
        public PaymentService(INotificationService notificationService, IQueryRepository<Account> accountQuery, IPersistenceRepository<PaymentOrder> paymentPersistence)
        {
            this.notificationService = notificationService;
            this.accountQuery = accountQuery;
            this.paymentPersistence = paymentPersistence;
        }

        public void ProcessPayment(PaymentOrder paymentOrder)
        {
            var account = accountQuery.SingleOrDefault(it => it.Id == paymentOrder.AccountId);

            if (account == null)
            {
                throw new InvalidOperationException("Account Not Found");
            }

            if (!string.IsNullOrWhiteSpace(paymentOrder.WebHookEndpoint) && string.IsNullOrWhiteSpace(paymentOrder.CorrelationId))
            {
                throw new InvalidOperationException("Webrook requires CorrelationId");
            }

            //Validar
            paymentOrder.Processed = false;

            this.paymentPersistence.Insert(paymentOrder);

            notificationService.Notity("payment_request", "", paymentOrder);

        }
    }
}
