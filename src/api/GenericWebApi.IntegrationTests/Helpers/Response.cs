namespace GenericWebApi.IntegrationTests.Helpers;

internal sealed record Response<T>
    (T Data,
    string[] Errors);

internal sealed record Response
    (string[] Errors);
