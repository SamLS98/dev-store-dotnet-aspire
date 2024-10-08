using DevStore.Identity.API.Data;
using DevStore.WebAPI.Core.DatabaseFlavor;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetDevPack.Identity.Jwt;
using NetDevPack.Security.PasswordHasher.Core;
using static DevStore.WebAPI.Core.DatabaseFlavor.ProviderConfiguration;

namespace DevStore.Identity.API.Configuration;

public static class IdentityConfig
{
    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache()
                .AddDataProtection();

        services.AddJwtConfiguration(configuration, "AppSettings")
                .AddNetDevPackIdentity<IdentityUser>()
                .PersistKeysToDatabaseStore<ApplicationDbContext>();

        services.AddIdentity<IdentityUser, IdentityRole>(o =>
            {
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireUppercase = false;
                o.Password.RequiredUniqueChars = 0;
                o.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.UpgradePasswordSecurity()
            .WithStrengthen(PasswordHasherStrength.Moderate)
            .UseArgon2<IdentityUser>();

        return services;
    }
}