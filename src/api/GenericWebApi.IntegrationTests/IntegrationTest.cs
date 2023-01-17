namespace GenericWebApi.IntegrationTests;

/// <summary>
/// Inherit from this class to create an integration test
/// </summary>
public class IntegrationTest
{
	public IntegrationTest()
	{
		WebApi = new WebApiApplication();
		TestClient = WebApi.CreateClient();
	}

    internal WebApiApplication WebApi { get; }

	protected HttpClient TestClient { get; }
}
