using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.FeatureManagement;
using BusinessLogic.Mapping;
using BusinessLogic.Options;
using BusinessLogic.Options.Configuration;
using DataAccess;
using DataAccess.Entities;
using GenericWebApi.Extensions;
using GenericWebApi.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllersWithViews().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
}); ;
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddDbContext<ApplicationContext>(o => 
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

services
    .AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();

services.AddOptions<JwtOptions>().BindConfiguration(JwtOptions.Section);
services.ConfigureOptions<MailSettingsSetup>();

services.AddBusinessLogicServices();
services.AddSwagger();

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new DefaultProfile());
    mc.AddProfile(new BusinessProfile());
});

services.AddSingleton(mapperConfig.CreateMapper());
services.AddSingleton<IConfigureOptions<MailSettingsOptions>, MailSettingsSetup>();

services.AddBearerAuthentication();

#region Features

services.AddFeatureManagement(builder.Configuration.GetSection("FeatureManagement"));

var featureManager = services.BuildServiceProvider().GetRequiredService<IFeatureManager>();

if (await featureManager.IsEnabledAsync(nameof(FeatureFlags.EmailVerification)))
{
    services.Configure<IdentityOptions>(opts =>
    {
        opts.SignIn.RequireConfirmedEmail = true;
    });
}

#endregion

var app = builder.Build();

await app.Services.CreateScope().ServiceProvider.GetRequiredService<ISeeder>().SeedAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
