using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentDemo.Manage.Entities;
using PaymentDemo.Manage.Models;
using PaymentDemo.Manage.Services.Abstractions;

namespace PaymentDemo.Api.Controllers.V1
{
    [ApiController]
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly IUserInfoService _userInfoService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserInfoService userInfoService, IMapper mapper, IUserService userService)
        {
            _userInfoService = userInfoService;
            _mapper = mapper;
            _userService = userService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> CreateUserManagement([FromBody] UserInfoViewModel _userData)
        {
            if (string.IsNullOrWhiteSpace(_userData.Password)
                || string.IsNullOrWhiteSpace(_userData.UserName))
                return BadRequest();

            var user = await _userInfoService.CreateUserInfo(_mapper.Map<UserInfo>(_userData));
            return Ok(user);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserViewModel user)
        {
            var result = await _userService.CreateUserAsync(user);
            if (result == 0) return BadRequest();

            return CreatedAtAction(nameof(Get), new { id = result }, result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _userService.GetUserAsync(id);
            if (result == null) return NotFound();

            return Ok(result);
        }
    }
}
