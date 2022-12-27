using BusinessLogic.Extensions;
using GenericWebApi.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;

namespace GenericWebApi.Controllers;

[Route("api/[controller]")]
public sealed class GoogleAuthController : ControllerBase
{
    private readonly IGoogleAuthService _googleService;

    public GoogleAuthController(IGoogleAuthService googleService)
    {
        _googleService = googleService;
        Code.Init();
    }

    [HttpPost("redirect-to-oauth")]
    public async Task<IActionResult> RedirectToOAuthServer()
    {
        var url = GetOAuthRequestUrl();

        return Redirect(url);
    }

    [HttpGet("redirect-to-oauth")]
    public async Task<IActionResult> GetLinkToOAuthServer()
    {
        string url = GetOAuthRequestUrl();

        return Ok(url);
    }

    [HttpGet("code")]
    public async Task<IActionResult> GetTokenResult([FromQuery] string code)
    {
        var redirectUri = $"{CurrentUrl}/api/GoogleAuth/code";
        var codeVerifier = Code.CodeVerifier;//HttpContext.Session.GetString("codeVerifier");

        var tokenResult = await _googleService.ExchangeCodeOnTokenAsync(code, codeVerifier, redirectUri);

        return Ok(tokenResult);
    }

    private string GetOAuthRequestUrl()
    {
        var scope = "https://www.googleapis.com/auth/userinfo.profile";
        var redirectUri = $"{CurrentUrl}/api/GoogleAuth/code";

        var codeVerifier = Code.CodeVerifier;//Guid.NewGuid().ToString();

        //HttpContext.Session.SetString("codeVerifier", codeVerifier);

        var hash = SHA256.HashData(codeVerifier.ToByteArray());
        var codeChallenge = Code.CodeChallenge;


        var url = _googleService.GenerateOAuthRequestUrl(scope, redirectUri, codeChallenge);
        return url;
    }

    private string CurrentUrl => $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

    public class Code
    {

        public static string CodeVerifier;

        public static string CodeChallenge;

        public static void Init()
        {
            CodeVerifier = GenerateNonce();
            CodeChallenge = GenerateCodeChallenge(CodeVerifier);
        }

        private static string GenerateNonce()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz123456789";
            var random = new Random();
            var nonce = new char[128];
            for (int i = 0; i < nonce.Length; i++)
            {
                nonce[i] = chars[i % chars.Length];
            }

            return new string(nonce);
        }

        private static string GenerateCodeChallenge(string codeVerifier)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
            var b64Hash = Convert.ToBase64String(hash);
            var code = Regex.Replace(b64Hash, "\\+", "-");
            code = Regex.Replace(code, "\\/", "_");
            code = Regex.Replace(code, "=+$", "");
            return code;
        }

    }
}


