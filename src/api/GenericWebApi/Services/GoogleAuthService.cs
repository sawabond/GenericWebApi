using GenericWebApi.Models.Google;
using GenericWebApi.Options;
using GenericWebApi.Services.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Policy;
using System.Web;

namespace GenericWebApi.Services;

public sealed class GoogleAuthService : IGoogleAuthService
{
    private const string OAuthServerEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
    private const string TokenEndpoint = "https://oauth2.googleapis.com/token";
    private readonly GoogleAuthOptions _options;

    public GoogleAuthService(IOptions<GoogleAuthOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateOAuthRequestUrl(string scope, string redirectUri, string codeChallenge)
    {
        var queryParams = new Dictionary<string, string>()
        {
            {"client_id", _options.ClientId},
            {"redirect_uri", redirectUri},
            {"response_type", "code" },
            {"scope", scope},
            {"code_challenge", codeChallenge},
            {"code_challenge_method", "S256"},
        };

        // TODO: refactor this part
        var uriBuilder = new UriBuilder(OAuthServerEndpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        queryParams.ToList().ForEach(kv =>
        {
            query[kv.Key] = kv.Value;
        });

        uriBuilder.Query = query.ToString();

        return uriBuilder.ToString();
    }

    public async Task<TokenResult> ExchangeCodeOnTokenAsync(string code, string codeVerifier, string redirectUri)
    {
        var queryParams = new Dictionary<string, string>()
        {
            {"client_id", _options.ClientId},
            {"client_secret", _options.ClientSecret},
            {"code", code },
            {"code_verifier", codeVerifier},
            {"grant_type", "authorization_code"},
            {"redirect_uri", redirectUri},
        };

        // TODO: refactor this part
        var uriBuilder = new UriBuilder(TokenEndpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        queryParams.ToList().ForEach(kv =>
        {
            query[kv.Key] = kv.Value;
        });

        uriBuilder.Query = query.ToString();

        var url = uriBuilder.ToString();

        var httpClient = new HttpClient();
        var response = await httpClient.PostAsync(url, null);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        var tokenResult = JsonConvert.DeserializeObject<TokenResult>(jsonResponse);

        return tokenResult;
    }

    public string RefreshToken(string refreshtoken)
    {
        return null;
    }
}
