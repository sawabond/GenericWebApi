namespace GenericWebApi.IntegrationTests.Helpers.Models;

internal sealed class LoginResponseModel
{
    public string Id { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string Token { get; set; }
}
