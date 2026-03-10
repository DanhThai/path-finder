using Common.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Repository
{
    public class QuestionConfig : IEntityTypeConfiguration<QuestionEntity>
    {
        public void Configure(EntityTypeBuilder<QuestionEntity> builder)
        {
            builder.BuildBaseEntity(TableName.Question);
            builder.HasOne(p => p.Course).WithMany(p => p.Questions)
                .HasForeignKey(p => p.CourseId);
        }
    }
}
