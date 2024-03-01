using FluentValidation;

namespace PaymentDemo.Manage.Models
{
    public class UserViewModel
    {
        public UserViewModel(int id, string firstName, string lastName, string phoneNumber, string? image, string? address)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Image = image;
            Address = address;            
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string? Image { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class UserViewModelValidator : AbstractValidator<UserViewModel>
    {
        public UserViewModelValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x=>x.PhoneNumber).NotEmpty();
        }
    }
}
