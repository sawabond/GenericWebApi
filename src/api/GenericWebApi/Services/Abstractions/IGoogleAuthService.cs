using GenericWebApi.Models.Google;

namespace GenericWebApi.Services.Abstractions;

public interface IGoogleAuthService
{
    Task<TokenResult> ExchangeCodeOnTokenAsync(string code, string codeVerifier, string redirectUrl);

    string GenerateOAuthRequestUrl(string scope, string redirectUrl, string codeChallenge);

    string RefreshToken(string refreshtoken);
}
