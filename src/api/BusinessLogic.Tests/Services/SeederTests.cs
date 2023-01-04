using BusinessLogic.Core;
using BusinessLogic.Services;
using BusinessLogic.Tests.Helpers;
using DataAccess.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;

namespace BusinessLogic.Tests.Services;

public sealed class SeederTests
{
    private const string DefaultPassword = "Pa$$w0rd";

    private readonly Seeder _seeder;
    private readonly Mock<UserManager<AppUser>> _userManager;
    private readonly Mock<RoleManager<AppRole>> _roleManager;

    public SeederTests()
	{
        _userManager = MockHelpers.TestUserManager<AppUser>();

        var userList = (new AppUser[] { Admin }).AsQueryable().BuildMock();
        _userManager.SetupGet(x => x.Users).Returns(userList);
        _userManager
            .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManager
            .Setup(x => x.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(Roles.AllowedRoles);

        _roleManager = MockHelpers.TestRoleManager<AppRole>();

        _seeder = new Seeder(_userManager.Object, _roleManager.Object);
    }

    private AppUser Admin => new AppUser
    {
        UserName = "Admin",
        NormalizedUserName = "ADMIN",
        Email = "some@mail.com",
        EmailConfirmed = true
    };

    private AppUser NotConfirmedEmailAdmin => new AppUser
    {
        UserName = "Admin",
        NormalizedUserName = "ADMIN",
        Email = "some@mail.com",
        EmailConfirmed = false
    };

    [Fact]
    public async void SeedAsync_CreatesAdminUser_IfAdminNotExists()
    {
        AdminDoesNotExist();

        var result = await _seeder.SeedAsync();

        _userManager.Verify(x => x.CreateAsync(It.Is<AppUser>(x => x.UserName == "Admin"), DefaultPassword));
    }

    [Fact]
    public async void SeedAsync_FailsIfUnableToCreateAdminUser_IfAdminNotExists()
    {
        AdminDoesNotExist();

        var result = await _seeder.SeedAsync();

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async void SeedAsync_ConfirmsEmail_IfNotConfirmed()
    {
        var admin = NotConfirmedEmailAdmin;
        var userList = (new AppUser[] { admin }).AsQueryable().BuildMock();
        _userManager.SetupGet(x => x.Users).Returns(userList);

        var result = await _seeder.SeedAsync();

        admin.EmailConfirmed.Should().BeTrue();
        _userManager.Verify(x => x.UpdateAsync(It.Is<AppUser>(x => x.UserName == "Admin")));
    }

    [Fact]
    public async void SeedAsync_SeedsRoles_IfRolesNotPresent()
    {
        _userManager
            .Setup(x => x.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(Array.Empty<string>());
        var notSeededRolesCount = Roles.AllowedRoles.Count();

        var result = await _seeder.SeedAsync();

        result.IsSuccess.Should().BeTrue();
        _roleManager.Verify(
            x => x.FindByIdAsync(It.IsIn(Roles.AllowedRoles)), 
            Times.Exactly(notSeededRolesCount));
        _roleManager.Verify(
            x => x.CreateAsync(It.Is<AppRole>(y => Roles.AllowedRoles.Contains(y.Name))), 
            Times.Exactly(notSeededRolesCount));
        _userManager.Verify(
            x => x.AddToRoleAsync(It.Is<AppUser>(y => y.UserName == "Admin"), It.IsIn(Roles.AllowedRoles)),
            Times.Exactly(notSeededRolesCount));
    }

    [Fact]
    public async void SeedAsync_SeedsOnlyMissingRoles_IfRolesNotPresent()
    {
        _userManager
            .Setup(x => x.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new string[] { Roles.Admin });
        var notSeededRolesCount = Roles.AllowedRoles.Count() - 1;

        var result = await _seeder.SeedAsync();

        result.IsSuccess.Should().BeTrue();
        _roleManager.Verify(
            x => x.FindByIdAsync(It.IsIn(Roles.AllowedRoles)),
            Times.Exactly(notSeededRolesCount));
        _roleManager.Verify(
            x => x.CreateAsync(It.Is<AppRole>(y => Roles.AllowedRoles.Contains(y.Name))),
            Times.Exactly(notSeededRolesCount));
        _userManager.Verify(
            x => x.AddToRoleAsync(It.Is<AppUser>(y => y.UserName == "Admin"), It.IsIn(Roles.AllowedRoles)),
            Times.Exactly(notSeededRolesCount));
    }

    private void AdminDoesNotExist()
    {
        var userList = (new List<AppUser>()).AsQueryable().BuildMock();
        _userManager.SetupGet(x => x.Users).Returns(userList);
        _userManager
            .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());
    }
}
