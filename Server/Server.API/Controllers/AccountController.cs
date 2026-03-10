using Common.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Service.Admin;

namespace Server.API
{
    [ApiVersion("1")]
    [AllowAnonymous]
    public class AccountController : APIBaseController
    {
        private readonly IAccountService _accountService;
        public AccountController(IHttpContextAccessor accessor, IAccountService accountService) : base(accessor)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<bool> Register([FromBody] RegisterDto dto)
        {
            return await _accountService.RegisterAsync(dto);
        }

        [HttpPost("login")]
        public async Task<AuthResponseDto> Login([FromBody] LoginDto dto)
        {
            return await _accountService.LoginAsync(dto);
        }

        [HttpPost("google-login")]
        public async Task<AuthResponseDto> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            return await _accountService.GoogleLoginAsync(dto);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<bool> Logout()
        {
            return await _accountService.LogoutAsync();
        }

        [HttpPost("forgot-password")]
        public async Task<string> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            return await _accountService.ForgotPasswordAsync(dto);
        }

        [HttpPost("reset-password")]
        public async Task<bool> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            return await _accountService.ResetPasswordAsync(dto);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<AccountDto> Me()
        {
            return await _accountService.GetProfileAsync(_httpContext.User);
        }
    }
}
