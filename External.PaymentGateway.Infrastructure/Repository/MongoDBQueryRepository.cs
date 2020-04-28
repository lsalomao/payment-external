using External.PaymentGateway.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace External.PaymentGateway.Repository
{
    public class MongoDBQueryRepository<T> : MongoDBRepository<T>, IQueryRepository<T>
        where T : IPersistentEntity
    {
        public MongoDBQueryRepository(IMongoCollection<T> collection): base(collection)
        {
        }

        public virtual T SingleOrDefault(Expression<Func<T, bool>> predicate) 
            => this.Collection.Find(predicate).SingleOrDefault();


    }
}
