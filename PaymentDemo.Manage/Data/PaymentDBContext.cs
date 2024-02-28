using Microsoft.EntityFrameworkCore;
using PaymentDemo.Manage.Entities;

namespace PaymentDemo.Manage.Data
{
    public class PaymentDBContext : DbContext
    {
        public PaymentDBContext(DbContextOptions<PaymentDBContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);            
            builder.Entity<ProductCategory>().HasKey(x => new { x.ProductId, x.CategoryId });
            builder.Entity<ProductCart>().HasKey(x => new { x.CartId, x.ProductId });

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<ProductCart> ProductCarts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
