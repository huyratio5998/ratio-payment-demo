namespace PaymentDemo.Manage.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string? Image { get; set; }
        public string? Address { get; set; }
    }
}
