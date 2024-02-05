using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mappings
{
	public class QueueMessageMapping : IEntityTypeConfiguration<QueueMessage>
	{
	    public void Configure(EntityTypeBuilder<QueueMessage> builder)
	    {
	        builder.ToTable("QueueMessage");

	        builder.HasKey(x => x.Id);

	        builder.Property(x => x.ExceptionMessage).HasMaxLength(512);
	        builder.Property(x => x.CreateDate).HasColumnType("datetime");
	        builder.Property(x => x.UpdateDate).HasColumnType("datetime");
        }
	}
}
