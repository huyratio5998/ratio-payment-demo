using PaymentDemo.Manage.Models;

namespace PaymentDemo.Manage.Services.Abstractions
{
    public interface IUserService
    {
        Task<List<UserViewModel>> GetUsersAsync();
        Task<UserViewModel?> GetUserAsync(int id);
        Task<int> CreateUserAsync(UserViewModel user);
        Task<bool> UpdateUserAsync(UserViewModel user);
    }
}
