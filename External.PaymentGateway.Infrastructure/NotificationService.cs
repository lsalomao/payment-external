using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace External.PaymentGateway
{
    public class NotificationService : INotificationService
    {
        private readonly IModel rabbitMQModel;

        public NotificationService(IModel rabbitMQModel)
        {
            this.rabbitMQModel = rabbitMQModel;
        }

        public void Notity(string exchange, string routingKey, object notification)
        {
            string serializedContent = System.Text.Json.JsonSerializer.Serialize(notification, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes(serializedContent);

            var prop = rabbitMQModel.CreateBasicProperties();

            rabbitMQModel.BasicPublish(exchange, routingKey, prop, messageBodyBytes);

        }
    }
}
