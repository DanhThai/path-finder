using Common.Domain;
using Microsoft.AspNetCore.Identity;

namespace Common.Repository
{
    public class AccountEntity : IdentityUser<Guid>
    {
        public string Name { get; set; }
        public CAccountType AccountType { get; set; }
        public CUserStatus Status { get; set; }
        public DateTimeOffset CreateAt { get; set; }
        public DateTimeOffset UpdateAt { get; set; }
        public Guid ModifiedBy { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
