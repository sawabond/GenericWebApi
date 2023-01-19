using FluentAssertions;
using GenericWebApi.IntegrationTests.Extensions;
using GenericWebApi.IntegrationTests.Helpers.Models;
using System.Net.Http.Json;

namespace GenericWebApi.IntegrationTests;

public class AuthControllerTests : IntegrationTest
{
	private const string Base = "api/auth";
	private const string Register = Base + "/register";
	private const string Login = Base + "/login";

    public AuthControllerTests() : base() { }

    [Fact]
	public async void Register_RegistersUser_WithValidData()
    {
        var content = GetJsonContent(RegisterModel);

        var response = await TestClient.PostAsync(Register, content);

        response.IsSuccessStatusCode.Should().BeTrue();
        (await response.AsErrors()).Should().BeEmpty();
    }

    [Fact]
    public async void Register_ReturnsFail_WhenUserExists()
    {
        var content = GetJsonContent(RegisterModel);

        var response1 = await TestClient.PostAsync(Register, content);
        var response = await TestClient.PostAsync(Register, content);

        response.IsSuccessStatusCode.Should().BeFalse();
		(await response.AsErrors()).Should().Contain("Username 'Username' is already taken.");
    }

    [Fact]
    public async void Register_ReturnsFail_WhenPasswordRequirementsNotMet()
    {
        var content = GetJsonContent(new { UserName = "Username", Password = "invalid", Email = "test@mail.com" });

        await TestClient.PostAsync(Register, content);
        var response = await TestClient.PostAsync(Register, content);

        response.IsSuccessStatusCode.Should().BeFalse();
        (await response.AsErrors()).Should().Contain("'Password' should be between 6 and 50 characters and contain " +
            "at least one lowercase letter, uppercase letter, special character and numeric symbol");
    }

    [Fact]
    public async void Login_ReturnsFail_WhenUserNotExists()
    {
        var content = GetJsonContent(LoginModel);

        var response = await TestClient.PostAsync(Login, content);

        response.IsSuccessStatusCode.Should().BeFalse();
        (await response.AsErrors()).Should().Contain("User with username Username was not found");
    }

    [Fact]
    public async void Login_ReturnsFail_WhenWrongPassword()
    {
        var registerContent = GetJsonContent(RegisterModel);
        await TestClient.PostAsync(Register, registerContent);
        var loginContent = GetJsonContent(new { Username = "Username", Password = "Wr0ngPa$$w0rd" });

        var response = await TestClient.PostAsync(Login, loginContent);

        response.IsSuccessStatusCode.Should().BeFalse();
        (await response.AsErrors()).Should().ContainEquivalentOf("Wrong password");
    }

    [Fact]
    public async void Login_ReturnsOk_WhenValidLogin()
    {
        var registerContent = GetJsonContent(RegisterModel);
        await TestClient.PostAsync(Register, registerContent);
        var loginContent = GetJsonContent(LoginModel);

        var response = await TestClient.PostAsync(Login, loginContent);

        response.IsSuccessStatusCode.Should().BeTrue();

        var content = await response.AsContent<LoginResponseModel>();
        content.Errors.Should().BeEmpty();

        var userModel = content.Data;
        userModel.UserName.Should().Be("Username");
        userModel.Email.Should().Be("test@mail.com");
        userModel.Id.Should().NotBeNullOrWhiteSpace();
        userModel.Token.Should().NotBeNullOrEmpty();
    }

    private static object RegisterModel 
        => new { UserName = "Username", Password = "Pa$$w0rd", Email = "test@mail.com" };

    private static object LoginModel
        => new { UserName = "Username", Password = "Pa$$w0rd", Email = "test@mail.com" };
}