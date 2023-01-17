namespace GenericWebApi.IntegrationTests.Helpers;

internal sealed record Response<T>
    (T Value,
    string[] Errors);

internal sealed record Response
    (string[] Errors);
