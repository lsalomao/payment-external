using External.PaymentGateway.Entities;
using MongoDB.Driver;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace External.PaymentGateway
{
    public class ApplicationInitializer
    {
        private readonly IMongoCollection<Account> collection;
        private readonly IModel rabbitMQModel;

        public ApplicationInitializer(IMongoCollection<Account> collection, RabbitMQ.Client.IModel rabbitMQModel)
        {
            this.collection = collection;
            this.rabbitMQModel = rabbitMQModel;
        }


        public void Initialize()
        {
            this.InitializeDatabase();
            this.InitializeRabbitMQ();
        }

        private void InitializeDatabase()
        {
            this.collection.Database.DropCollection("Account");
            this.collection.Database.DropCollection("PaymentOrder");

            this.collection.InsertOne(new Account { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Active = true, Name = "Conta 1" });
            this.collection.InsertOne(new Account { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Active = true, Name = "Conta 2" });
            this.collection.InsertOne(new Account { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Active = true, Name = "Conta 3" });
            this.collection.InsertOne(new Account { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Active = true, Name = "Conta 4" });
        }

        private void InitializeRabbitMQ()
        {
            rabbitMQModel.QueueDeclare("payments", true, false, false);
            rabbitMQModel.QueueDeclare("payments_history", true, false, false);
            rabbitMQModel.QueueDeclare("notification", true, false, false);

            rabbitMQModel.ExchangeDeclare("payment_request", "topic", true, false);

            rabbitMQModel.QueueBind("payments", "payment_request", "");
            rabbitMQModel.QueueBind("payments_history", "payment_request", "");


        }

    }
}
