using AutoMapper;
using BusinessLogic.Mapping;
using BusinessLogic.Options;
using DataAccess;
using DataAccess.Entities;
using GenericWebApi.Extensions;
using GenericWebApi.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

services.AddBusinessLogicServices();

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new DefaultProfile());
    mc.AddProfile(new BusinessProfile());
});

services.AddSingleton(mapperConfig.CreateMapper());

services.AddBearerAuthentication();

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
