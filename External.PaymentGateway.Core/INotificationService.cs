namespace External.PaymentGateway
{
    public interface INotificationService
    {
        void Notity(string exchange, string routingKey, object notification);
    }
}