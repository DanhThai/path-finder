using Common.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Repository
{
    public class MyCourseConfig : IEntityTypeConfiguration<MyCourseEntity>
    {
        public void Configure(EntityTypeBuilder<MyCourseEntity> builder)
        {
            builder.BuildBaseEntity(TableName.MyCourse);
            builder.HasOne(p => p.Learner).WithMany().HasForeignKey(p => p.UserId);
            builder.HasOne(p => p.Course).WithMany(p => p.EnrolledLearners)
                .HasForeignKey(p => p.CourseId);
        }
    }
}
