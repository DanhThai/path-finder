using System.Security.Claims;
using Common.Domain;
using Common.Service;

namespace Server.Service.Admin
{
    public interface IAccountService : IScopeDependency
    {
        Task<bool> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto model);
        Task<AuthResponseDto> GoogleLoginAsync(GoogleLoginDto model);
        Task<bool> LogoutAsync();
        Task<string> ForgotPasswordAsync(ForgotPasswordDto model);
        Task<bool> ResetPasswordAsync(ResetPasswordDto model);
        Task<AccountDto> GetProfileAsync(ClaimsPrincipal user);
    }
}
