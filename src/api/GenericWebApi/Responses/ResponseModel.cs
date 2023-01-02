namespace GenericWebApi.Responses;

public record ResponseModel<T>
    (T Data,
    string[] Errors);

public record ResponseModel
    (string[] Errors);

public record PagingResponseModel<T>
    (T Data,
    string[] Errors,
    int PageNumber,
    int PageSize);