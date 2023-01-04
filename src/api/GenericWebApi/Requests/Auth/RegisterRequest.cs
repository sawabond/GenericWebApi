namespace GenericWebApi.Requests.Auth;

public record RegisterRequest
    (string UserName,
    string Password,
    string Email);
