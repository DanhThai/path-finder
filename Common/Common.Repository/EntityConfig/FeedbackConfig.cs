using Common.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Repository
{
    public class FeedbackConfig : IEntityTypeConfiguration<FeedBackEntity>
    {
        public void Configure(EntityTypeBuilder<FeedBackEntity> builder)
        {
            builder.BuildBaseEntity(TableName.Feedback);
            builder.HasOne(p => p.LearnerTask).WithMany(p => p.FeedBacks).HasForeignKey(p => p.LearnerTaskId);
        }
    }
}
