using Common.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Repository
{
    public class UserQuizAttempConfig : IEntityTypeConfiguration<UserQuizAttempEntity>
    {
        public void Configure(EntityTypeBuilder<UserQuizAttempEntity> builder)
        {
            builder.BuildBaseEntity(TableName.UserQuizAttemp);
            builder.HasOne(p => p.MyCourse).WithOne().HasForeignKey<UserQuizAttempEntity>(p => p.MyCourseId);
        }
    }
}
