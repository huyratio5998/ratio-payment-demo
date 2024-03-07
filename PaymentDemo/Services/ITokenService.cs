using PaymentDemo.Api.Models;
using PaymentDemo.Manage.Entities;

namespace PaymentDemo.Api.Services
{
    public interface ITokenService
    {
        TokenResponse CreateToken(UserInfo user);
    }
}
