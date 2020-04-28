using System;
using System.Collections.Generic;
using System.Text;

namespace External.PaymentGateway.Entities
{
    public interface IPersistentEntity

    {        
        Guid Id { get; set; }
    }
}
