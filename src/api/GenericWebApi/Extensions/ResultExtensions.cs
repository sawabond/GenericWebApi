using AutoFilterer.Types;
using FluentResults;
using GenericWebApi.Responses;

namespace GenericWebApi.Extensions;

public static class ResultExtensions
{
    public static IEnumerable<string> ToErrors<T>(this Result<T> @this) =>
        @this.Errors.Select(e => e.Message);

    public static IEnumerable<string> ToErrors(this Result @this) =>
        @this.Errors.Select(e => e.Message);

    public static ResponseModel<T> ToResponse<T>(this Result<T> @this) =>
        new ResponseModel<T>(@this.ValueOrDefault, @this.ToErrors().ToArray());

    public static ResponseModel ToResponse(this Result @this) =>
        new ResponseModel(@this.ToErrors().ToArray());

    public static PagingResponseModel<T> ToResponse<T>(this Result<T> @this, PaginationFilterBase filter) =>
        new PagingResponseModel<T>(@this.ValueOrDefault, @this.ToErrors().ToArray(), filter.Page, filter.PerPage);
}
