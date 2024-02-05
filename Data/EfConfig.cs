using Core.Entities;
using Core.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Data
{
	public class EfConfig
	{
		public static void Configure(ModelBuilder builder)
		{
		    builder.Entity<AppRole>().ToTable("AppRole").Ignore(x => x.ConcurrencyStamp);
            builder.Entity<IdentityUserRole<long>>().ToTable("AppUserRole");
            builder.Entity<IdentityUserLogin<long>>().ToTable("AppUserLogin").Ignore(x=>x.ProviderDisplayName);
            builder.Entity<IdentityUserClaim<long>>().ToTable("AppUserClaim");
		    builder.Entity<IdentityRoleClaim<long>>().ToTable("AppRoleClaim");
		    builder.Ignore<IdentityUserToken<long>>();
		    builder.Entity<UserToken>().ToTable("UserToken");
		    builder.Entity<UserDevice>().ToTable("UserDevice");
        }
    }
}
