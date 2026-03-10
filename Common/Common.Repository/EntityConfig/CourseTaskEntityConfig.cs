using Common.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Repository
{
    public class CourseTaskEntityConfig : IEntityTypeConfiguration<CourseTaskEntity>
    {
        public void Configure(EntityTypeBuilder<CourseTaskEntity> builder)
        {
            builder.BuildBaseEntity(TableName.CourseTask);
            builder.HasOne(p => p.Course).WithMany(p => p.Tasks).HasForeignKey(p => p.CourseId);
        }
    }
}
