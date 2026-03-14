using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Domain;
using Common.Repository;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Server.Service.Admin
{
    public class AccountService(
        UserManager<AccountEntity> _userManager,
        IDBRepository _repository
        ) : IAccountService
    {

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var user = new AccountEntity
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true,
                AccountType = CAccountType.Learner,
                CreateAt = DateTimeOffset.UtcNow,
                ModifiedAt = DateTimeOffset.UtcNow,
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                throw new WarningHandleException($"Failed to register new account. {createResult.Errors.Select(e => e.Description)}");
            }

            var learnerProfile = new LearnerProfileEntity
            {
                Id = user.Id,
                LearnerID = dto.LearnerID,
                MajorId = dto.MajorId,
            };
            await _repository.AddAsync(learnerProfile);

            return true;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var user = await _userManager.FindByEmailAsync(dto.Username)
                ?? throw new NotExistException("Account");

            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                throw new DataValidationException("Username or password is incorrect", "", CErrorCode.InvalidInput);
            }

            if (user.Status != CUserStatus.Active)
            {
                throw new DataValidationException("Account is inactive", "", CErrorCode.StatusNotSupport);
            }

            return new AuthResponseDto
            {
                AccessToken = GenerateJwtToken(user)
            };
        }

        public async Task<AuthResponseDto> GoogleLoginAsync(GoogleLoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.IdToken))
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new[] { RuntimeContext.Config.Authentication.Google.ClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken, settings);
            var email = payload.Email;
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new AccountEntity
                {
                    Id = Guid.NewGuid(),
                    Name = payload.Name,
                    Email = email,
                    UserName = email,
                    AccountType = CAccountType.Learner,
                    EmailConfirmed = true,
                    Status = CUserStatus.Active,
                    CreateAt = DateTimeOffset.UtcNow,
                    ModifiedAt = DateTimeOffset.UtcNow
                };

                await _repository.ActionInTransaction(async () =>
                {
                    var createResult = await _userManager.CreateAsync(user, "Student@123");
                    if (!createResult.Succeeded)
                    {
                        throw new WarningHandleException($"Failed to login google account. {createResult.Errors.Select(e => e.Description)}");
                    }

                    var profile = new LearnerProfileEntity
                    {
                        Id = user.Id,
                    };

                    await _repository.AddAsync(profile);
                });


                await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", payload.Subject, "Google"));
                await _userManager.ConfirmEmailAsync(user, await _userManager.GenerateEmailConfirmationTokenAsync(user));
            }

            return new AuthResponseDto
            {
                AccessToken = GenerateJwtToken(user)
            };
        }

        public Task<bool> LogoutAsync()
        {
            // With JWT, logout is handled client-side by discarding the token
            return Task.FromResult(true);
        }


        public async Task<string> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var user = await _userManager.FindByEmailAsync(dto.Email) ?? throw new NotExistException("Account");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return token;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var user = await _userManager.FindByEmailAsync(dto.Email) ?? throw new NotExistException("Account");

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
            {
                throw new WarningHandleException("Cannot reset password");
            }

            return true;
        }

        public async Task<AccountDto> GetProfileAsync(ClaimsPrincipal userPrincipal)
        {
            var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new NotExistException("Account");

            var user = await _userManager.FindByIdAsync(userId) ?? throw new UnauthorizedAccessException();

            return new AccountDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Name = user.Name,
                Avatar = user.Avatar,
                AccountType = user.AccountType,
                Status = user.Status
            };
        }

        private string GenerateJwtToken(AccountEntity user)
        {
            var jwtConfig = RuntimeContext.Config.Authentication.JWT;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name ?? user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtConfig.Issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(5),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
