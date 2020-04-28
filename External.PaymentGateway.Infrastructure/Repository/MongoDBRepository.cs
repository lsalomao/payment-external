using External.PaymentGateway.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace External.PaymentGateway.Repository
{
    public abstract class MongoDBRepository<T>
        where T : IPersistentEntity
    {
        protected IMongoCollection<T> Collection { get; }

        public MongoDBRepository(IMongoCollection<T> collection)
        {
            this.Collection = collection;
        }

    }
}
