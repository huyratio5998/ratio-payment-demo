namespace PaymentDemo.Manage.Entities
{
    public class Product: BaseEntity
    {
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Number { get; set; }
        public string? Image { get; set; }

        public List<ProductCategory> ProductCategories { get; set; }
        public List<ProductCart>? ProductCarts { get; set; }
    }
}
