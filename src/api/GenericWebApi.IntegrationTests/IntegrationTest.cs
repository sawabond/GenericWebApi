using GenericWebApi.IntegrationTests.Extensions;
using GenericWebApi.IntegrationTests.Helpers.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Json;

namespace GenericWebApi.IntegrationTests;

/// <summary>
/// Inherit from this class to create an integration test
/// </summary>
[Collection("Sequential")]
public class IntegrationTest
{
	public IntegrationTest()
	{
		WebApi = new WebApiApplication();
		TestClient = WebApi.CreateClient();
	}

    internal WebApiApplication WebApi { get; }

	protected HttpClient TestClient { get; }

	protected async Task Authorize(string username, string password)
	{
		var response = await TestClient.PostAsync(
			"api/auth/login",
            GetJsonContent(new { username, password }));

		var token = (await response.AsContent<LoginResponseModel>()).Data.Token;

        TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

	protected async Task AuthorizeAsAdmin() => await Authorize("Admin", "Pa$$w0rd");

    protected JsonContent GetJsonContent(object @object) =>
        JsonContent.Create(@object);
}
