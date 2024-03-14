namespace PaymentDemo.Manage.Enums
{
    public enum PaymentType
    {
        COD = 0,
        VISA = 1,
    }

    public enum PaymentProvider
    {
        Paypal = 0,
        Adyen = 1,
        InternalBank = 2       
    }

    public enum PaymentRequestType
    {
        RequestPayment = 0,
        Refund = 1
    }
}
