using External.PaymentGateway.Entities;

namespace External.PaymentGateway
{
    public interface IPaymentService
    {
        void ProcessPayment(PaymentOrder paymentOrder);
    }
}