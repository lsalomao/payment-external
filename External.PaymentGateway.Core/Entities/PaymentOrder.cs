using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace External.PaymentGateway.Entities
{
    public class PaymentOrder: IPersistentEntity
    {
        [BsonId(IdGenerator = typeof(CombGuidGenerator))]
        public Guid Id { get; set; }

        public string CorrelationId { get; set; }

        public Guid AccountId { get; set; }

        public decimal Amount { get; set; }

        public string CardNumber { get; set; }

        public int CVC { get; set; }

        public DateTime Validate { get; set; }

        public string Name { get; set; }

        public string WebHookEndpoint { get; set; }

        public bool Processed { get; set; }

    }
}
