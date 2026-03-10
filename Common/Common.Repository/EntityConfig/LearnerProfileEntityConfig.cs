using Common.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Repository
{
    public class LearnerProfileEntityConfig : IEntityTypeConfiguration<LearnerProfileEntity>
    {
        public void Configure(EntityTypeBuilder<LearnerProfileEntity> builder)
        {
            builder.ToTable(TableName.LearnerProfile);
            builder.HasOne(p => p.Account).WithOne().HasForeignKey<LearnerProfileEntity>(p => p.Id);
        }
    }
}
