using External.PaymentGateway.Entities;
using External.PaymentGateway.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace External.PaymentGateway
{
    public class ProcessService
    {
        private readonly IQueryRepository<Account> accountQueryRepository;

        private readonly IPersistenceRepository<PaymentOrder> paymentPersistence;
        private readonly IPersistenceRepository<Account> accountPersistence;
        private readonly IConsumer<PaymentOrder> consumer;

        public ProcessService(
            IQueryRepository<Account> accountQueryRepository,
            IPersistenceRepository<PaymentOrder> paymentPersistence,
            IPersistenceRepository<Account> accountPersistence,
            IConsumer<PaymentOrder> consumer
            )
        {
            this.accountQueryRepository = accountQueryRepository;
            this.paymentPersistence = paymentPersistence;
            this.accountPersistence = accountPersistence;
            this.consumer = consumer;
        }


        public void Consume()
        {
            this.consumer.Start("payments", this.ProcessPayment);
        }

        private void ProcessPayment(PaymentOrder payment)
        {
            var account = accountQueryRepository.SingleOrDefault(it => it.Id == payment.AccountId);

            if (account == null)
            {
                throw new InvalidOperationException("Account Not found");
            }

            if (payment.CVC == 12)
            {
                throw new Exception();
            }

            account.Balance += payment.Amount;
            payment.Processed = true;

            this.paymentPersistence.Update(payment);

            this.accountPersistence.Update(account);

            if (!string.IsNullOrWhiteSpace(payment.WebHookEndpoint))
            {
                //TODO: Abstrair
                HttpClient client = new HttpClient();

                StringContent stringContent = new StringContent(payment.CorrelationId);

                client.PostAsync($"{payment.WebHookEndpoint}", stringContent).GetAwaiter().GetResult();
            }
        }
    }
}
