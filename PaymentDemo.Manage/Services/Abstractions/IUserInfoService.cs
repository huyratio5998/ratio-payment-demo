using PaymentDemo.Manage.Entities;

namespace PaymentDemo.Manage.Services.Abstractions
{
    public interface IUserInfoService
    {
        Task<UserInfo?> GetUserAsync(string email, string password);
    }
}
