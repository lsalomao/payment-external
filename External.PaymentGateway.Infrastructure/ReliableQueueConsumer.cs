using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Polly;

namespace External.PaymentGateway
{
    public class ReliableQueueConsumer<T> : QueueConsumer<T>, IConsumer<T>
    {
        private readonly int retryCount;

        public ReliableQueueConsumer(IModel rabbitMQModel, int retryCount) : base(rabbitMQModel)
        {
            this.retryCount = retryCount;
        }

        protected override void Dispatch(T treatedObject, Action<T> action)
        {
            var policy = Policy
              .Handle<Exception>()
              .WaitAndRetry(retryCount, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
              );

            policy.Execute(() =>
            {
                action(treatedObject);
            });
        }

    }
}
