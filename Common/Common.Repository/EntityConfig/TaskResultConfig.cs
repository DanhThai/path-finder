using Common.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Repository
{
    public class TaskResultConfig : IEntityTypeConfiguration<TaskResultEntity>
    {
        public void Configure(EntityTypeBuilder<TaskResultEntity> builder)
        {
            builder.BuildBaseEntity(TableName.TaskResult);
            builder.HasOne(p => p.MyCourse).WithMany(p => p.TaskResults)
                .HasForeignKey(p => p.MyCourseId);
            builder.HasOne(p => p.CourseTask).WithMany().HasForeignKey(p => p.TaskId);
        }
    }
}
