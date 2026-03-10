using Common.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Repository
{
    public class LearnerMajorEntityConfig : IEntityTypeConfiguration<LearnerMajorEntity>
    {
        public void Configure(EntityTypeBuilder<LearnerMajorEntity> builder)
        {
            builder.BuildBaseEntity(TableName.LearnerMajor);
        }
    }
}
