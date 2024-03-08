using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentDemo.Api.Services;
using PaymentDemo.Manage.Entities;
using PaymentDemo.Manage.Models;
using PaymentDemo.Manage.Services.Abstractions;

namespace PaymentDemo.Api.Controllers.V1
{
    [ApiController]
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly IUserInfoService _userInfoService;
        private readonly ITokenService _tokenService;

        public TokenController(IUserInfoService userInfoService, ITokenService tokenService)
        {
            _userInfoService = userInfoService;
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserInfoViewModel _userData)
        {
            if (_userData != null && _userData.UserName != null && _userData.Password != null)
            {
                var user = await _userInfoService.GetUserAsync(_userData.UserName, _userData.Password);
                if (user == null) return BadRequest();

                var token = _tokenService.CreateToken(user);
                return Ok(token);
            }

            return BadRequest();
        }
    }
}
