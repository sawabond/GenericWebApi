using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.Mapping;
using BusinessLogic.Models.AppUser;
using BusinessLogic.Services;
using BusinessLogic.Tests.Helpers;
using DataAccess.Entities;
using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;

namespace BusinessLogic.Tests.Services;

public class GoogleAuthServiceTests
{
    private readonly GoogleAuthService _googleAuthService;
    private readonly Mock<UserManager<AppUser>> _userManager;
    private readonly Mock<SignInManager<AppUser>> _signInManager;
    private readonly Mock<ClaimsPrincipal> _claims;
    private readonly Mock<ITokenService> _tokenService;

    public GoogleAuthServiceTests()
    {
        _userManager = MockHelpers.TestUserManager<AppUser>();

        _userManager
            .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManager
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(User);

        _signInManager = MockHelpers.TestSignInManager<AppUser>();

        _claims = new Mock<ClaimsPrincipal>();
        _claims
            .Setup(x => x.FindFirst(ClaimTypes.Email))
            .Returns(new Claim(ClaimTypes.Email, User.Email));

        _signInManager
            .Setup(x => x.GetExternalLoginInfoAsync(null))
            .ReturnsAsync(new ExternalLoginInfo(
                _claims.Object,
                "google",
                "providerKey",
                "google"));
        _signInManager
            .Setup(x => x.ExternalLoginSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, true))
            .ReturnsAsync(SignInResult.Success);

        _tokenService = new Mock<ITokenService>();

        _tokenService
            .Setup(x => x.CreateTokenAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(Result.Ok("Some valid jwt"));

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<BusinessProfile>();
        });

        var mapper = config.CreateMapper();

        _googleAuthService = new GoogleAuthService(
            _signInManager.Object,
            _userManager.Object,
            mapper,
            _tokenService.Object);
    }

    private AppUser User => new AppUser { Id = Guid.Empty.ToString(), UserName = "name", Email = "email" };
    private UserAuthModel AuthModel => new UserAuthModel
    {
        Id = Guid.Empty.ToString(),
        UserName = "name",
        Email = "email",
        Token = "Some valid jwt",
    };

    [Fact]
    public async void LoginUserAsync_ReturnsFail_IfExternalLoginInfoIsNull()
    {
        _signInManager
            .Setup(x => x.GetExternalLoginInfoAsync(null))
            .ReturnsAsync(null as ExternalLoginInfo);

        var result = await _googleAuthService.LoginUserAsync();

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainEquivalentOf(new Error("Error while loading external login information"));
    }

    [Fact]
    public async void LoginUserAsync_ReturnsOk_IfSignInResultSucceed()
    {
        var result = await _googleAuthService.LoginUserAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(AuthModel);
    }

    [Fact]
    public async void LoginUserAsync_ReturnsFail_IfUserNotRegisteredAndEmailIsNull()
    {
        _signInManager
            .Setup(x => x.ExternalLoginSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, true))
            .ReturnsAsync(SignInResult.Failed);
        _claims
            .Setup(x => x.FindFirst(ClaimTypes.Email))
            .Returns(null as Claim);

        var result = await _googleAuthService.LoginUserAsync();

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainEquivalentOf(new Error("Could not get email data"));
    }

    [Fact]
    public async void LoginUserAsync_RegistersUser_IfNotPresent()
    {
        _signInManager
            .Setup(x => x.ExternalLoginSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, true))
            .ReturnsAsync(SignInResult.Failed);
        _userManager
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(null as AppUser);

        var result = await _googleAuthService.LoginUserAsync();

        result.IsSuccess.Should().BeTrue();
        _userManager.Verify(x => x.CreateAsync(It.IsAny<AppUser>()));
    }

    [Fact]
    public async void LoginUserAsync_AddsExternalLoginAndSignsIn_IfUserIsRegistered()
    {
        SetupExternalSignInFailedAndUserFoundByEmail();

        var result = await _googleAuthService.LoginUserAsync();

        result.IsSuccess.Should().BeTrue();
        _userManager.Verify(x => x.AddLoginAsync(It.IsAny<AppUser>(), It.IsAny<ExternalLoginInfo>()));
        _signInManager.Verify(x => x.SignInAsync(It.IsAny<AppUser>(), false, null));
    }

    [Fact]
    public async void LoginUserAsync_ReturnsFail_IfUnableToCreateToken()
    {
        SetupExternalSignInFailedAndUserFoundByEmail();
        _tokenService
            .Setup(x => x.CreateTokenAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(Result.Fail("Token errors"));

        var result = await _googleAuthService.LoginUserAsync();

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainEquivalentOf(new Error("Token errors"));
    }

    [Fact]
    public async void LoginUserAsync_ReturnsOkWithAuthResult_IfExternalSignInSuccessful()
    {
        SetupExternalSignInFailedAndUserFoundByEmail();

        var result = await _googleAuthService.LoginUserAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(AuthModel);
    }

    [Fact]
    public async void ConfigureExternalAuthenticationProperties_CallsSignInManagerMethod()
    {
        var result = await _googleAuthService.ConfigureExternalAuthenticationProperties("redirect url");

        result.IsSuccess.Should().BeTrue();
        _signInManager.Verify(x => x.ConfigureExternalAuthenticationProperties("google", "redirect url", null));
    }

    private void SetupExternalSignInFailedAndUserFoundByEmail()
    {
        _signInManager
                    .Setup(x => x.ExternalLoginSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, true))
                    .ReturnsAsync(SignInResult.Failed);
        _userManager
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(User);
    }
}
