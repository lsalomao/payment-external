using External.PaymentGateway.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace External.PaymentGateway.Repository
{
    public class MongoDBPersistenceRepository<T> : MongoDBRepository<T>, IPersistenceRepository<T>
        where T : IPersistentEntity
    {

        public MongoDBPersistenceRepository(IMongoCollection<T> collection) : base(collection)
        {
        }

        public virtual long Delete(T entity) => this.Collection.DeleteOne(it => it.Id == entity.Id).DeletedCount;

        public virtual void Insert(T entity) => this.Collection.InsertOne(entity);

        public virtual void Update(T entity) =>
            this.Collection.ReplaceOne(it => it.Id == entity.Id, entity);

    }
}
