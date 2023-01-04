using BusinessLogic.Enums;
using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationContext(
        this IServiceCollection services, 
        string connString,
        DatabaseType databaseType) =>
        services.AddDbContext<ApplicationContext>(o =>
        {
            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                    o.UseSqlServer(connString, x => x.MigrationsAssembly("SqlServerMigrations"));
                    break;
                case DatabaseType.PostgreSql:
                    o.UseNpgsql(connString); //, x => x.MigrationsAssembly("PostgresMigrations")
                    break;
                default:
                    break;
            }
        });

    public static IdentityBuilder AddApplicationIdentity(this IServiceCollection services) =>
        services
        .AddIdentity<AppUser, AppRole>()
        .AddEntityFrameworkStores<ApplicationContext>()
        .AddDefaultTokenProviders();
}
