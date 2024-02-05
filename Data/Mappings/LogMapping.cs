using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mappings
{
    public class LogMapping : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.ToTable("Log");
            builder.HasKey(x => new { x.Id, x.ExecutionTime });

            builder.Property(x => x.EventDateTime).HasColumnType("datetime");
            builder.Property(x => x.Type).HasMaxLength(512);
            builder.Property(x => x.Path).HasMaxLength(512);
            builder.Property(x => x.RequestType).HasMaxLength(512);
            builder.Property(x => x.ErrorSource).HasMaxLength(512);
        }
    }
}
