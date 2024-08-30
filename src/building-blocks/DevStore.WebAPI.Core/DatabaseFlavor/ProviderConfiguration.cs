using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;

namespace DevStore.WebAPI.Core.DatabaseFlavor;

public class ProviderConfiguration(string connString)
{
    public ProviderConfiguration With() => this;
    private static readonly string MigrationAssembly = typeof(ProviderConfiguration).GetTypeInfo().Assembly.GetName().Name;

    public static ProviderConfiguration Build(string connString)
    {
        return new ProviderConfiguration(connString);
    }

    public Action<DbContextOptionsBuilder> SqlServer =>
        options => options.UseSqlServer(connString, sql => sql.MigrationsAssembly(MigrationAssembly));

    public Action<DbContextOptionsBuilder> MySql =>
        options => options.UseMySql(connString, ServerVersion.AutoDetect(connString), sql => sql.MigrationsAssembly(MigrationAssembly));

    public Action<DbContextOptionsBuilder> Postgre =>
        options =>
        {
            options.UseNpgsql(connString, sql => sql.MigrationsAssembly(MigrationAssembly));
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        };

    public Action<DbContextOptionsBuilder> Sqlite =>
        options => options.UseSqlite(connString, sql => sql.MigrationsAssembly(MigrationAssembly));


    /// <summary>
    /// it's just a tuple. Returns 2 parameters.
    /// Trying to improve readability at ConfigureServices
    /// </summary>
    public static (DatabaseType, string) DetectDatabase(IConfiguration configuration) => (
        configuration.GetValue<DatabaseType>("AppSettings:DatabaseType", DatabaseType.None),
        configuration.GetConnectionString("DefaultConnection"));
}