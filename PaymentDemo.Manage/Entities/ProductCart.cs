namespace PaymentDemo.Manage.Entities
{
    public class ProductCart : BaseEntity
    {
        public int Number { get; set; }
        public decimal Price { get; set; }

        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}