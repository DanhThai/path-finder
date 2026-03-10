using Common.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Repository
{
    public class StorageDocumentEntityConfig : IEntityTypeConfiguration<StorageDocumentEntity>
    {
        public void Configure(EntityTypeBuilder<StorageDocumentEntity> builder)
        {
            builder.ToTable(TableName.StorageDocument);
        }
    }
}
