using Microsoft.IdentityModel.Tokens;
using PaymentDemo.Api.Models;
using PaymentDemo.Manage.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PaymentDemo.Api.Services
{
    public class TokenService : ITokenService
    {
        private ILogger<TokenService> _logger;
        private readonly IConfiguration _configuration;        

        public TokenService(ILogger<TokenService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public TokenResponse CreateToken(UserInfo user)
        {
            var expiration = DateTime.UtcNow.AddMinutes(DemoConstant.JWTExpirationMinutes);
            var token = CreateJwtToken(
                CreateClaims(user),
                CreateSigningCredentials(),
                expiration
            );
            var tokenHandler = new JwtSecurityTokenHandler();

            _logger.LogInformation("JWT Token created");

            var result = new TokenResponse
            {
                Token = tokenHandler.WriteToken(token),
                Expiration = token.ValidTo
            };
            return result;
        }

        private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials,
            DateTime expiration) =>
            new(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expiration,
                signingCredentials: credentials
            );

        private List<Claim> CreateClaims(UserInfo user)
        {
            var jwtSub = _configuration["Jwt:Subject"];

            try
            {
                var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwtSub),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim("Id", user.Id.ToString()),
                new Claim("DisplayName", user.DisplayName),
                new Claim("UserName", user.UserName),
                new Claim("Email", user.Email),                
            };
                if (user.UserName.Equals(DemoConstant.AdminAccount)) claims.Add(new Claim(ClaimTypes.Role, DemoConstant.RatioAdmin));
                else claims.Add(new Claim(ClaimTypes.Role, DemoConstant.RatioReadOnly));

                return claims;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private SigningCredentials CreateSigningCredentials()
        {
            var symmetricSecurityKey = _configuration["Jwt:Key"];

            return new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(symmetricSecurityKey)
                ),
                SecurityAlgorithms.HmacSha256
            );
        }
    }
}
