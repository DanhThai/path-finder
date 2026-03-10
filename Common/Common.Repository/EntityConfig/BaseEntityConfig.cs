using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Repository
{
    public static class BaseConfig
    {
        public static void BuildBaseEntity<T>(this EntityTypeBuilder<T> builder, string tableName) where T : BaseEntity
        {
            builder.ToTable(tableName);
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
        }
    }
}
