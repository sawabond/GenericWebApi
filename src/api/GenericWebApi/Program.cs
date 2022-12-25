using AutoMapper;
using BusinessLogic;
using BusinessLogic.Mapping;
using BusinessLogic.Models;
using BusinessLogic.Options;
using DataAccess;
using DataAccess.Entities;
using GenericWebApi.Extensions;
using GenericWebApi.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationContext>(o => 
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services
    .AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();

builder.Services.AddOptions<JwtOptions>().BindConfiguration("Jwt");

builder.Services.Scan(selector => selector
                .FromAssemblies(typeof(AssemblyReference).Assembly)
                .AddClasses(filter => filter.NotInNamespaceOf<ModelsNamespaceReference>(), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new DefaultProfile());
    mc.AddProfile(new BusinessProfile());
});

builder.Services.AddSingleton(mapperConfig.CreateMapper());

builder.Services.AddBearerAuthentication();

var app = builder.Build();

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
