using External.PaymentGateway.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace External.PaymentGateway.Repository
{
    public interface IPersistenceRepository<T>
        where T : IPersistentEntity
    {

        void Insert(T entity);

        void Update(T entity);

        long Delete(T entity);

    }
}
