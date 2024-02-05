using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mappings
{
    public class EFMigrationsHistoryMapping : IEntityTypeConfiguration<EFMigrationsHistory>
    {
        public void Configure(EntityTypeBuilder<EFMigrationsHistory> builder)
        {
            builder.ToTable("EFMigrationsHistory");

            builder.HasKey(x => x.MigrationId);
            builder.Property(x => x.MigrationId).HasDefaultValue(false).HasMaxLength(255);
        }
    }
}
