using BusinessLogic.Enums;
using BusinessLogic.Options;
using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendGrid;

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
                    o.UseNpgsql(connString, x => x.MigrationsAssembly("PostgresMigrations"));
                    break;
                case DatabaseType.InMemory:
                    o.UseInMemoryDatabase("InMemory");
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

    public static IServiceCollection AddSendGridClient(this IServiceCollection services, IConfiguration config) =>
        services.AddSingleton<ISendGridClient>(_ =>
        {
            var apiKey = config.GetValue<string>("MailSettingsOptions:SendGridKey");
            return new SendGridClient(apiKey);
        });
}
