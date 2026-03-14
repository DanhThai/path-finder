using Common.Domain;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Repository
{
    public class AccountEntity : IdentityUser<Guid>
    {
        public string Name { get; set; }
        public CAccountType AccountType { get; set; }
        public CUserStatus Status { get; set; }

        [Column(TypeName = "jsonb")]
        public DocumentProperty Avatar { get; set; }
        public DateTimeOffset CreateAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public Guid ModifiedBy { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
