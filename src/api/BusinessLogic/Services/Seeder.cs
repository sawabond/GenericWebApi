using BusinessLogic.Abstractions;
using DataAccess.Entities;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services;

internal sealed class Seeder : ISeeder
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public Seeder(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Result> SeedAsync()
    {
        return await SeedAdminAndRoles();
    }

    private async Task<Result> SeedAdminAndRoles()
    {
        var admin = await _userManager.Users
                    .Where(u => u.NormalizedUserName == "ADMIN")
                    .FirstOrDefaultAsync();

        if (admin is null)
        {
            admin = new AppUser { UserName = "Admin", Email = "some@mail.com", EmailConfirmed = true };

            var adminResult = await _userManager.CreateAsync(admin, "Pa$$w0rd");

            if (adminResult.Succeeded == false)
            {
                return Result.Fail("Unable to seed admin user");
            }
        }

        admin.EmailConfirmed = true;
        await _userManager.UpdateAsync(admin);

        var roles = await _userManager.GetRolesAsync(admin);

        var expectedRoles = new string[] { "Admin", "User", "Moder" };

        foreach (var expectedRole in expectedRoles)
        {
            if (roles.Contains(expectedRole))
            {
                continue;
            }

            var role = await _roleManager.FindByIdAsync(expectedRole);

            if (role is null)
            {
                await _roleManager.CreateAsync(new AppRole { Name = expectedRole });
            }

            await _userManager.AddToRoleAsync(admin, expectedRole);
        }

        return Result.Ok();
    }
}
