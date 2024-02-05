using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mappings
{
	public class UserMapping : IEntityTypeConfiguration<User>
	{
	    public void Configure(EntityTypeBuilder<User> builder)
	    {
	       builder.ToTable("User");

	        builder.HasKey(x => x.Id);
            builder.Property(x => x.FbToken).HasMaxLength(256);
	        builder.Ignore(x => x.TwoFactorEnabled);
	        builder.Ignore(x => x.AccessFailedCount);
	        builder.Ignore(x => x.ConcurrencyStamp);
	        builder.Ignore(x => x.NormalizedEmail);
	        builder.Ignore(x => x.NormalizedUserName);
	    }
	}
}
