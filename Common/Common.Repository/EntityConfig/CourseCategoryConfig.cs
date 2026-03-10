using Common.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Repository
{
    public class CourseCategoryConfig : IEntityTypeConfiguration<CourseCategoryEntity>
    {
        public void Configure(EntityTypeBuilder<CourseCategoryEntity> builder)
        {
            builder.BuildBaseEntity(TableName.CourseCategory);
        }
    }
}
