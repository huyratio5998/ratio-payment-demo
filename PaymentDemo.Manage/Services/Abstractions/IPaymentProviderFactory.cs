using PaymentDemo.Manage.Enums;

namespace PaymentDemo.Manage.Services.Abstractions
{
    public interface IPaymentProviderFactory
    {
        IPaymentProvider? CreatePaymentProvider(PaymentProvider? paymentProvider);
    }
}
