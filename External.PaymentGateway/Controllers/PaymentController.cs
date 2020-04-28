using System;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RabbitMQ.Client;
using MongoDB.Bson.IO;
using External.PaymentGateway.Entities;
using External.PaymentGateway.Repository;

namespace External.PaymentGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IQueryRepository<PaymentOrder> paymentQuery;
        private readonly IPaymentService paymentService;

        public PaymentController(
            IQueryRepository<PaymentOrder> paymentQuery,
            IPaymentService paymentService)
        {
            this.paymentQuery = paymentQuery;
            this.paymentService = paymentService;
        }

        [HttpGet("info")]
        public string GetInfo() => DateTime.Now.ToString();


        [HttpGet("{id:guid}")]
        public PaymentOrder Get(Guid id)
        {
            PaymentOrder returnValue = this.paymentQuery.SingleOrDefault(it => it.Id == id);
            return returnValue;
        }


        [HttpPost()]
        public IActionResult Process([FromBody]PaymentOrder paymentOrder)
        {
            
            this.paymentService.ProcessPayment(paymentOrder);

            return this.Ok();
        }

    }
}
