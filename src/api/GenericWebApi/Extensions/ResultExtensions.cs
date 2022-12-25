using FluentResults;

namespace GenericWebApi.Extensions;

public static class ResultExtensions
{
    public static string ToErrorString<T>(this Result<T> @this) =>
        string.Join(", ", @this.Errors.Select(e => e.Message));

    public static string ToErrorString(this Result @this) =>
        string.Join(", ", @this.Errors.Select(e => e.Message));
}
