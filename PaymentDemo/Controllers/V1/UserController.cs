using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentDemo.Manage.Entities;
using PaymentDemo.Manage.Services.Abstractions;

namespace PaymentDemo.Api.Controllers.V1
{
    [ApiController]
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserInfoService _userInfoService;

        public UserController(IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserInfo _userData)
        {
            if (string.IsNullOrWhiteSpace(_userData.Password)                
                || string.IsNullOrWhiteSpace(_userData.UserName))
                return BadRequest();

            var user = await _userInfoService.CreateUserInfo(_userData);
            return Ok(user);
        }
    }
}
