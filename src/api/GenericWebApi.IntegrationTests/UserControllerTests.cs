using BusinessLogic.Models.AppUser;
using FluentAssertions;
using GenericWebApi.IntegrationTests.Extensions;
using GenericWebApi.IntegrationTests.Helpers.Models;
using System.Net;

namespace GenericWebApi.IntegrationTests;

public class UserControllerTests : IntegrationTest
{
    private const string Base = "api/user";
    private const string Current = Base + "/current";

    public UserControllerTests() : base() { }

    private static object User =>
        new { UserName = "Username", Password = "Pa$$w0rd", Email = "test@mail.com" };

    [Fact]
    public async void Endpoints_RequireAuthentication()
    {
        var responses = new List<HttpResponseMessage>();

        responses.Add(await TestClient.GetAsync(Base));
        responses.Add(await TestClient.GetAsync(Current));
        responses.Add(await TestClient.GetAsync($"{Base}/{Guid.NewGuid()}"));
        responses.Add(await TestClient.PostAsync(Base, GetJsonContent(User)));
        responses.Add(await TestClient.PatchAsync($"{Base}/{Guid.NewGuid()}", GetJsonContent(User)));
        responses.Add(await TestClient.DeleteAsync($"{Base}/{Guid.NewGuid()}"));

        responses.Should().AllSatisfy(x => x.StatusCode.Should().Be(HttpStatusCode.Unauthorized));
    }

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
        var id = Guid.NewGuid();

        var response = await TestClient.GetAsync($"{Base}/{id}");

        response.IsSuccessStatusCode.Should().BeFalse();
        (await response.AsErrors()).Should().ContainEquivalentOf($"User with id {id} was not found");
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
    public async void GetAllUsers_ReturnsOkWithFilteredUsers_WhenFilterApplied()
    {
        await AuthorizeAsAdmin();
        await RegisterTestUsers();
        
        var response = await TestClient.GetAsync($"{Base}?userName.stw=Username0");

        response.IsSuccessStatusCode.Should().BeTrue();
        var users = (await response.AsContent<IEnumerable<UserResponseModel>>()).Data;
        users.Should().HaveCount(1);
        users.Should().Contain(x => x.UserName == "Username0" && x.Email == "some0@mail.com");
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

    [Fact]
    public async void PatchUser_ReturnsFail_WhenUserNotFound()
    {
        await AuthorizeAsAdmin();
        var id = Guid.NewGuid();

        var response = await TestClient.PatchAsync($"{Base}/{id}", GetJsonContent(User));

        response.IsSuccessStatusCode.Should().BeFalse();
        (await response.AsErrors()).Should().ContainEquivalentOf($"User with id {id} was not found");
    }

    [Fact]
    public async void PatchUser_ReturnsOk_WhenUserPatched()
    {
        await AuthorizeAsAdmin();
        var createResult = await TestClient.PostAsync($"{Base}?role=User", GetJsonContent(User));
        var createdId = (await createResult.AsContent<string>()).Data;


        var patchResponse = await TestClient.PatchAsync($"{Base}/{createdId}", 
            GetJsonContent(new { Username = "PatchedUsername", Email = "patched@mail.com" }));
        var userResponse = await TestClient.GetAsync($"{Base}/{createdId}");

        patchResponse.IsSuccessStatusCode.Should().BeTrue();
        var user = (await userResponse.AsContent<UserRegisterModel>()).Data;
        user.UserName.Should().Be("PatchedUsername");
        user.Email.Should().Be("patched@mail.com");
    }

    [Fact]
    public async void DeleteUser_ReturnsOk_WhenUserDeleted()
    {
        await AuthorizeAsAdmin();
        var createResult = await TestClient.PostAsync($"{Base}?role=User", GetJsonContent(User));
        var createdId = (await createResult.AsContent<string>()).Data;

        var userResponse = await TestClient.GetAsync($"{Base}/{createdId}");
        var deleteResponse = await TestClient.DeleteAsync($"{Base}/{createdId}");
        var deletedUserResponse = await TestClient.GetAsync($"{Base}/{createdId}");

        userResponse.IsSuccessStatusCode.Should().BeTrue();
        deleteResponse.IsSuccessStatusCode.Should().BeTrue();
        deletedUserResponse.IsSuccessStatusCode.Should().BeFalse();
    }

    [Fact]
    public async void DeleteUser_ReturnsFail_WhenUserNotFound()
    {
        await AuthorizeAsAdmin();
        var id = Guid.NewGuid();

        var response = await TestClient.DeleteAsync($"{Base}/{id}");

        response.IsSuccessStatusCode.Should().BeFalse();
        (await response.AsErrors()).Should().ContainEquivalentOf($"User with id {id} was not found");
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
