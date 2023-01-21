using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.Enums;
using BusinessLogic.Extensions;
using BusinessLogic.FeatureManagement;
using BusinessLogic.Mapping;
using BusinessLogic.Options;
using BusinessLogic.Options.Configuration;
using GenericWebApi.Extensions;
using GenericWebApi.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;

        services.AddControllersWithViews().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddCors(c =>
        {
            c.AddPolicy("DefaultPolicy", p =>
            {
                p.AllowAnyMethod();
                p.AllowAnyOrigin();
                p.AllowAnyHeader();
            });
        });

        services.AddApplicationContext(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            builder.Configuration.GetValue<DatabaseType>("DatabaseType"));
        services.AddApplicationIdentity();
        services.AddSendGridClient(builder.Configuration);

        services.AddOptions<JwtOptions>().BindConfiguration(JwtOptions.Section);

        services.AddBusinessLogicServices();
        services.AddSwagger();

        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new DefaultProfile());
            mc.AddProfile(new BusinessProfile());
        });

        services.AddSingleton(mapperConfig.CreateMapper());
        services.AddSingleton<IConfigureOptions<MailSettingsOptions>, MailSettingsSetup>();

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

        if (await featureManager.IsEnabledAsync(nameof(FeatureFlags.GoogleAuthentication)))
        {
            services.AddOptions<GoogleAuthOptions>().BindConfiguration(GoogleAuthOptions.Section);

            services.AddBearerAuthentication().AddGoogle("google", opt =>
            {
                var gogleOptions = services.BuildServiceProvider().GetService<IOptions<GoogleAuthOptions>>().Value;
                opt.ClientId = gogleOptions.ClientId;
                opt.ClientSecret = gogleOptions.ClientSecret;
                opt.SignInScheme = IdentityConstants.ExternalScheme;
            });
        }
        else
        {
            services.AddBearerAuthentication();
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

        app.UseCors("DefaultPolicy");

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}