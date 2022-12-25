namespace GenericWebApi.Responses;

public record ResponseModel<T>
    (T Data,
    string[] Errors);

public record ResponseModel
    (string[] Errors);