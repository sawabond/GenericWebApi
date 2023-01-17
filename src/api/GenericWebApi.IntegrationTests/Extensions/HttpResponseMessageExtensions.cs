using GenericWebApi.IntegrationTests.Helpers;
using Newtonsoft.Json;

namespace GenericWebApi.IntegrationTests.Extensions;

internal static class HttpResponseMessageExtensions
{
    public static async Task<Response<T>> AsContent<T>(this HttpResponseMessage @this) =>
        JsonConvert.DeserializeObject<Response<T>>(await @this.Content.ReadAsStringAsync());

    public static async Task<Response> AsContent(this HttpResponseMessage @this) =>
        JsonConvert.DeserializeObject<Response>(await @this.Content.ReadAsStringAsync());

    public static async Task<string[]> AsErrors<T>(this HttpResponseMessage @this) =>
        JsonConvert.DeserializeObject<Response<T>>(await @this.Content.ReadAsStringAsync()).Errors;

    public static async Task<string[]> AsErrors(this HttpResponseMessage @this) =>
        JsonConvert.DeserializeObject<Response>(await @this.Content.ReadAsStringAsync()).Errors;
}
