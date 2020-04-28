using System;
using System.Collections.Generic;
using System.Text;

namespace External.PaymentGateway
{
    public interface IConsumer<T>
    {
        void Start(string queueName, Action<T> action);
    }
}
