using Common.Domain;
using Common.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Domain.Admin;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Server.Service.Admin
{
    public class AccountManagementService(
        IDBRepository _repository,
        UserManager<AccountEntity> _userManager
        ) : IAccountManagementService
    {
        public async Task<AccountDto> GetDetail(Guid id)
        {
            var account = await _repository.GetSet<AccountEntity>(p => p.Id == id).FirstOrDefaultAsync()
                ?? throw new NotExistException("Account");

            return new AccountDto
            {
                Id = account.Id,
                Name = account.Name,
                Username = account.UserName,
                Email = account.Email,
                AccountType = account.AccountType,
                Status = account.Status,
                ModifiedAt = account.ModifiedAt,
                Avatar = account.Avatar
            };
        }

        public async Task<TableInfo<AccountDto>> GetPaging(CTableParameter parameter)
        {
            var query = new TableQueryParameter<AccountEntity>
            {
                Pager = new Pager { PageIndex = parameter.PageIndex, PageSize = parameter.PageSize },
                Sorter = GenerateSorter(parameter),
                Filter = GenerateFilter(parameter)
            };

            return await _repository.GetWithPagingAsync(query, account => new AccountDto
            {
                Id = account.Id,
                Name = account.Name,
                Username = account.UserName,
                Email = account.Email,
                AccountType = account.AccountType,
                Status = account.Status,
                ModifiedAt = account.ModifiedAt,
            });
        }

        public async Task<bool> Add(SaveAccountDto dto)
        {
            var entity = new AccountEntity
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true,
                AccountType = dto.AccountType,
                CreateAt = DateTimeOffset.UtcNow,
                ModifiedAt = DateTimeOffset.UtcNow
            };

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new WarningHandleException("Password is required");
            }

            await _repository.ActionInTransaction(async () =>
            {
                var result = await _userManager.CreateAsync(entity, dto.Password);
                if (!result.Succeeded)
                {
                    throw new WarningHandleException($"Can't create account: {string.Join(",", result.Errors.Select(s => s.Description))}");
                }

                if (dto.AccountType == CAccountType.Learner)
                {
                    var learnerProfile = new LearnerProfileEntity
                    {
                        Id = entity.Id,
                    };
                    await _repository.AddAsync(learnerProfile);
                }
            });


            return true;
        }

        public async Task<bool> Activate(Guid id)
        {
            var account = await _repository.GetSet<AccountEntity>(p => p.Id == id).FirstOrDefaultAsync()
                ?? throw new NotExistException("Account");

            account.Status = CUserStatus.Active;

            await _repository.UpdateAsync(account);
            return true;
        }

        public async Task<bool> Update(Guid id, SaveAccountDto dto)
        {
            var account = await _repository.GetSet<AccountEntity>(p => p.Id == id).FirstOrDefaultAsync()
                ?? throw new NotExistException("Account");

            account.Name = dto.Name;

            await _repository.UpdateAsync(account);
            return true;
        }

        public async Task<bool> UpdateMyProfile(ClaimsPrincipal userPrincipal, AccountDto dto)
        {
            var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? throw new NotExistException("Account");

            var account = await _userManager.FindByIdAsync(userId)
                ?? throw new NotExistException("Account");

            account.Name = dto.Name;
            if (dto.Avatar != null)
            {
                account.Avatar = dto.Avatar;
            }

            await _repository.UpdateAsync(account);
            return true;
        }

        public async Task<bool> Deactivate(Guid id)
        {
            if (id == RuntimeContext.Current.UserId)
            {
                throw new WarningHandleException("Can not deactivate your account");
            }

            var account = await _repository.GetSet<AccountEntity>(p => p.Id == id).FirstOrDefaultAsync()
                ?? throw new NotExistException("Account");

            account.Status = CUserStatus.Inactive;

            await _repository.UpdateAsync(account);
            return true;
        }

        #region Private methods
        private Expression<Func<AccountEntity, bool>> GenerateFilter(CTableParameter param)
        {
            var filter = PredicateBuilder.True<AccountEntity>();

            if (!string.IsNullOrWhiteSpace(param.SearchContent))
            {
                var content = param.SearchContent.ToLower().Trim();
                filter = filter.And(p => p.Name.ToLower().Contains(content) || p.Email.ToLower().Contains(content));
            }

            if (param.Filters != null)
            {
                if (param.Filters.TryGetValue(nameof(AccountDto.Status), out var statusValues) && statusValues.Count == 1)
                {
                    var status = (CUserStatus)Enum.Parse(typeof(CUserStatus), statusValues[0]);
                    filter = filter.And(p => p.Status == status);
                }

                if (param.Filters.TryGetValue(nameof(AccountDto.AccountType), out var typeValues) && typeValues.Count == 1)
                {
                    var type = (CAccountType)Enum.Parse(typeof(CAccountType), typeValues[0]);
                    filter = filter.And(p => p.AccountType == type);
                }
            }

            return filter;
        }

        private Sorter<AccountEntity, object> GenerateSorter(CTableParameter param)
        {
            var result = new Sorter<AccountEntity, object> { IsAscending = param.IsAscending };

            switch ((param.SortKey ?? "").ToLower())
            {
                case "name":
                    result.SortBy = s => s.Name;
                    break;
                case "accounttype":
                    result.SortBy = s => s.AccountType;
                    break;
                case "status":
                    result.SortBy = s => s.Status;
                    break;
                case "modifiedat":
                    result.SortBy = s => s.ModifiedAt;
                    break;
                default:
                    result.SortBy = s => s.Name;
                    break;
            }

            return result;
        }
        #endregion
    }
}
