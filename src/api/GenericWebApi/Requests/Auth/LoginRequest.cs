namespace GenericWebApi.Requests.Auth;

public sealed record LoginRequest
    (string UserName,
    string Password);
