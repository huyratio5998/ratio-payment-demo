using FluentValidation;

namespace PaymentDemo.Manage.Models
{
    public class ProductViewModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Number { get; set; }        
        public string? Image { get; set; }
        public IFormFile? UploadedImage { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<CategoryViewModel>? ProductCategories { get; set; }        
    }
    public class ProductViewModelValidator : AbstractValidator<ProductViewModel>
    {
        public ProductViewModelValidator() {
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Number).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Name).NotNull().NotEmpty();

            RuleForEach(x => x.ProductCategories)
                .ChildRules(c =>
                {                    
                    c.RuleFor(x => x.Id).GreaterThan(0);
                });
        }
    }
}
