using FluentAssertions;
using GenericWebApi.IntegrationTests.Extensions;
using System.Net.Http.Json;

namespace GenericWebApi.IntegrationTests;

public class AuthControllerTests : IntegrationTest
{
	private const string Base = "api/auth";
	private const string Register = Base + "/register";

    public AuthControllerTests() : base()
	{

	}

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

    private static JsonContent GetJsonContent(object @object) =>
        JsonContent.Create(@object);

    private static object RegisterModel => new { UserName = "Username", Password = "Pa$$w0rd", Email = "test@mail.com" };
}