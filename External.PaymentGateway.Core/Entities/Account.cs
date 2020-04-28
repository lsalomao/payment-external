using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace External.PaymentGateway.Entities
{
    public class Account: IPersistentEntity
    {
        [BsonId(IdGenerator = typeof(CombGuidGenerator))]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }


        public decimal Balance { get; set; }
    }
}
