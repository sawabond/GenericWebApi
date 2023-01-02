using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.Mapping;
using BusinessLogic.Models.AppUser;
using BusinessLogic.Services;
using BusinessLogic.Validation.Abstractions;
using DataAccess.Entities;
using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace BusinessLogic.Tests.Services;

public sealed class AuthServiceTests
{
	private readonly AuthService _authService;
	private readonly Mock<UserManager<AppUser>> _userManager;
	private readonly Mock<SignInManager<AppUser>> _signInManager;
	private readonly Mock<ITokenService> _tokenService;
	private readonly Mock<IMailService> _mailService;
	private readonly Mock<IModelValidator> _validator;

	public AuthServiceTests()
	{
		_userManager = MockHelpers.TestUserManager<AppUser>();

		_userManager
			.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(User);
        _userManager
            .Setup(x => x.CheckPasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        _signInManager = MockHelpers.TestSignInManager<AppUser>();

        _signInManager
            .Setup(x => x.CheckPasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), false))
            .ReturnsAsync(SignInResult.Success);

        _tokenService = new Mock<ITokenService>();

		_tokenService.Setup(x => x.CreateTokenAsync(It.IsAny<AppUser>())).ReturnsAsync(Result.Ok("Some valid jwt"));

		_mailService = new Mock<IMailService>();
		_validator = new Mock<IModelValidator>();

		_validator.Setup(v => v.Validate(It.IsAny<It.IsAnyType>())).Returns(Result.Ok());

		var config = new MapperConfiguration(cfg =>
		{
			cfg.AddProfile<BusinessProfile>();
		});

		var mapper = config.CreateMapper();

		_authService = new AuthService(
			_userManager.Object,
			_signInManager.Object,
			mapper,
			_tokenService.Object,
			_validator.Object,
			_mailService.Object);
	}

	private UserLoginModel LoginModel => new UserLoginModel("name", "pass");
	private UserRegisterModel RegisterModel => new UserRegisterModel("name", "pass", "email");
	private AppUser User => new AppUser { Id = Guid.Empty.ToString(), UserName = "name", Email = "email" };
	private UserAuthModel AuthModel => new UserAuthModel
	{
		UserName = "name",
		Email = "email",
		Token = "Some valid jwt",
		Id = Guid.Empty.ToString()
	};


    [Fact]
	public async void Login_ReturnsFailedResultWithErrors_WhenModelIsNotValid()
	{
		_validator
			.Setup(x => x.Validate(It.IsAny<UserLoginModel>()))
			.Returns(Result.Fail("Validation errors"));

		var result = await _authService.LoginAsync(LoginModel);

		result.IsSuccess.Should().BeFalse();
		result.Errors.Should().ContainEquivalentOf(new Error("Validation errors"));
	}

	[Fact]
	public async void Login_ReturnsUserNotFound_IfUserNotFound()
	{
		_userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(null as AppUser);

		var result = await _authService.LoginAsync(LoginModel);

		result.IsSuccess.Should().BeFalse();
		result.Errors.Should().ContainEquivalentOf(new Error($"User with username {LoginModel.UserName} was not found"));
	}

    [Fact]
    public async void Login_ReturnsWrongPassword_IfPasswordIsWrong()
    {
		_userManager
			.Setup(x => x.CheckPasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
			.ReturnsAsync(false);

        var result = await _authService.LoginAsync(LoginModel);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainEquivalentOf(new Error("Wrong password"));
    }

    [Fact]
    public async void Login_ReturnsInvalidLoginAttempt_IfUnableToLogin()
    {
		_signInManager
			.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), false))
			.ReturnsAsync(SignInResult.Failed);

        var result = await _authService.LoginAsync(LoginModel);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainEquivalentOf(new Error("Invalid login attempt"));
    }

    [Fact]
    public async void Login_ReturnsTokenErrors_IfUnableToCreateToken()
    {
        _tokenService.Setup(x => x.CreateTokenAsync(It.IsAny<AppUser>())).ReturnsAsync(Result.Fail("Token error"));

        var result = await _authService.LoginAsync(LoginModel);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainEquivalentOf(new Error("Token error"));

    }

    [Fact]
    public async void Login_ReturnsAuthModel_IfLoginSucceed()
    {
        var result = await _authService.LoginAsync(LoginModel);

        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
		result.Value.Should().BeEquivalentTo(AuthModel);
    }

    [Fact]
    public async void Register_ReturnsFailedResultWithErrors_WhenModelIsNotValid()
    {
        var model = 
        _validator
            .Setup(x => x.Validate(It.IsAny<UserRegisterModel>()))
            .Returns(Result.Fail("Validation errors"));

        var result = await _authService.RegisterAsync(RegisterModel);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainEquivalentOf(new Error("Validation errors"));
    }
}
