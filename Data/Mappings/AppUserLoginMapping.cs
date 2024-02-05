using Core.IdentityEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mappings
{
    public class AppUserLoginMapping : IEntityTypeConfiguration<AppUserLogin>
    {
        public void Configure(EntityTypeBuilder<AppUserLogin> builder)
        {
            builder.ToTable("AppUserLogin");

            builder.Property(x => x.LoginProvider).HasMaxLength(255);
            builder.Property(x => x.ProviderKey).HasMaxLength(255);
        }
    }
}
