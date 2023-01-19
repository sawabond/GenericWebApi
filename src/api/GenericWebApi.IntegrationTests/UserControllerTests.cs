using BusinessLogic.Models.AppUser;
using FluentAssertions;
using GenericWebApi.IntegrationTests.Extensions;
using GenericWebApi.IntegrationTests.Helpers.Models;

namespace GenericWebApi.IntegrationTests;

public class UserControllerTests : IntegrationTest
{
    private const string Base = "api/user";
    private const string Current = Base + "/current";

    public UserControllerTests() : base() { }

    private static object User =>
        new { UserName = "Username", Password = "Pa$$w0rd", Email = "test@mail.com" };

    [Fact]
    public async void GetCurrentUser_ReturnsCurrentUser_WhenAuthorized()
    {
        await AuthorizeAsAdmin();

        var response = await TestClient.GetAsync(Current);

        response.IsSuccessStatusCode.Should().BeTrue();
        var user = (await response.AsContent<UserResponseModel>()).Data;
        user.UserName.Should().Be("Admin");
    }

    [Fact]
    public async void GetUserById_ReturnsFail_WhenUserNotFound()
    {
        await AuthorizeAsAdmin();

        var response = await TestClient.GetAsync($"{Base}/{Guid.Empty}");

        response.IsSuccessStatusCode.Should().BeFalse();
        (await response.AsErrors()).Should().ContainEquivalentOf($"User with id {Guid.Empty} was not found");
    }

    [Fact]
    public async void GetUserById_ReturnsOkWithUser_WhenUserFound()
    {
        await AuthorizeAsAdmin();
        var createResult = await TestClient.PostAsync($"{Base}?role=User", GetJsonContent(User));
        var createdId = (await createResult.AsContent<string>()).Data;

        var response = await TestClient.GetAsync($"{Base}/{createdId}");

        response.IsSuccessStatusCode.Should().BeTrue();
        var user = (await response.AsContent<UserResponseModel>()).Data;
        user.Email.Should().Be("test@mail.com");
        user.UserName.Should().Be("Username");
    }

    [Fact]
    public async void GetAllUsers_ReturnsOkWithUsers_WhenUsersPresent()
    {
        await AuthorizeAsAdmin();
        await RegisterTestUsers();

        var response = await TestClient.GetAsync(Base);

        response.IsSuccessStatusCode.Should().BeTrue();
        var users = (await response.AsContent<IEnumerable<UserResponseModel>>()).Data;
        users.Should().Contain(x => x.UserName == "Username0" && x.Email == "some0@mail.com");
        users.Should().Contain(x => x.UserName == "Username1" && x.Email == "some1@mail.com");
        users.Should().Contain(x => x.UserName == "Username2" && x.Email == "some2@mail.com");
    }

    [Fact]
	public async void CreateUser_ReturnsOk_WhenUserCreated()
	{
        await AuthorizeAsAdmin();

        var response = await TestClient.PostAsync($"{Base}?role=User", GetJsonContent(User));

        response.IsSuccessStatusCode.Should().BeTrue();
        (await response.AsContent<string>()).Should().NotBeNull();
    }

    [Fact]
    public async void CreateUser_ReturnsFail_WhenUserAlreadyExists()
    {
        await AuthorizeAsAdmin();

        await TestClient.PostAsync($"{Base}?role=User", GetJsonContent(User));
        var response = await TestClient.PostAsync($"{Base}?role=User", GetJsonContent(User));

        response.IsSuccessStatusCode.Should().BeFalse();
        (await response.AsErrors()).Should().ContainEquivalentOf("The user already exists");
    }

    private async Task RegisterTestUsers()
    {
        await AuthorizeAsAdmin();

        for (var i = 0; i < 3; i++)
        {
            await TestClient.PostAsync($"{Base}?role=User", 
                GetJsonContent(new { Username = $"Username{i}", Password = "Pa$$w0rd", Email = $"some{i}@mail.com"}));
        }
    }
}
