using Common.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Repository
{
    public class CourseConfig : IEntityTypeConfiguration<CourseEntity>
    {
        public void Configure(EntityTypeBuilder<CourseEntity> builder)
        {
            builder.BuildBaseEntity(TableName.Course);
            builder.HasOne<CourseCategoryEntity>().WithMany().HasForeignKey(p => p.CategoryId);
        }
    }
}
