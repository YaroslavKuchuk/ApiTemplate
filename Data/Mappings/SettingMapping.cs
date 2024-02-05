using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mappings
{
	public class SettingMapping : IEntityTypeConfiguration<Setting>
	{
	    public void Configure(EntityTypeBuilder<Setting> builder)
	    {
	        builder.ToTable("Setting");
	        builder.HasKey(x => x.Id);
	        builder.Property(x => x.ParamName).HasMaxLength(512);
        }
	}
}
