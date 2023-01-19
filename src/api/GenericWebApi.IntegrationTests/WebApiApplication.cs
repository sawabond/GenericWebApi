using DataAccess;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GenericWebApi.IntegrationTests;

public class WebApiApplication : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddJsonFile("appsettings.Development.json").AddEnvironmentVariables().Build();
        });

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
