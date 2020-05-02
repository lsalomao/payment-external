using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace External.PaymentGateway
{
    public class QueueConsumer<T>: IConsumer<T>
    {
        private readonly IModel rabbitMQModel;

        public QueueConsumer(IModel rabbitMQModel)
        {
            this.rabbitMQModel = rabbitMQModel;
        }

        public void Start(string queueName, Action<T> action)
        {
            var consumer = new EventingBasicConsumer(rabbitMQModel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();

                var message = Encoding.UTF8.GetString(body);

                T treatedObject = default(T);

                try
                {
                    treatedObject = System.Text.Json.JsonSerializer.Deserialize<T>(message);
                }
                catch (Exception ex)
                {
                    rabbitMQModel.BasicReject(ea.DeliveryTag, true);
                    throw;
                }

                try
                {
                    Dispatch(treatedObject, action);
                    rabbitMQModel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception)
                {
                    rabbitMQModel.BasicNack(ea.DeliveryTag, false, true);
                    throw;
                }

            };


            rabbitMQModel.BasicConsume(queue: queueName,
                                 autoAck: false,
                                 consumer: consumer);
        }

        protected virtual void Dispatch(T treatedObject, Action<T> action) => action(treatedObject);

    }
}
