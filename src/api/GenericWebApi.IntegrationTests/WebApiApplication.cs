using DataAccess;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Respawn;
using System.Data.Common;

namespace GenericWebApi.IntegrationTests;

public class WebApiApplication : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddScoped(sp =>
            {
                return new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase("InMemory")
                .UseApplicationServiceProvider(sp)
                .Options;
            });

            var context = services.BuildServiceProvider().GetRequiredService<ApplicationContext>();
            context.Database.EnsureDeleted();
        });

        return base.CreateHost(builder);
    }
}
