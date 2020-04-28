using External.PaymentGateway.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace External.PaymentGateway.Repository
{
    public interface IQueryRepository<T>
        where T : IPersistentEntity
    {
        T SingleOrDefault(Expression<Func<T, bool>> predicate);
        
    }
}
