using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationContext(this IServiceCollection services, string connString) =>
        services.AddDbContext<ApplicationContext>(o =>
        {
            o.UseSqlServer(connString);
        });

    public static IdentityBuilder AddApplicationIdentity(this IServiceCollection services) =>
        services
        .AddIdentity<AppUser, AppRole>()
        .AddEntityFrameworkStores<ApplicationContext>()
        .AddDefaultTokenProviders();
}
