using BusinessLogic.Abstractions;
using GenericWebApi.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace GenericWebApi.Controllers;

[Route("api/[controller]")]
public sealed class GoogleAuthController : ControllerBase
{
    private readonly IGoogleAuthService _googleService;

    public GoogleAuthController(
        IGoogleAuthService googleService)
    {
        _googleService = googleService;
    }

    [HttpGet("login")]
    public async Task<IActionResult> GoogleLogin([FromQuery] string returnUrl)
    {
        
        var redirectUrl = $"{Url.Action(nameof(ExternalLoginCallback), "GoogleAuth", new {returnUrl})}";
        var propertiesResult = await _googleService.ConfigureExternalAuthenticationProperties(redirectUrl);

        return propertiesResult.IsSuccess
            ? Challenge(propertiesResult.Value, "google")
            : BadRequest(propertiesResult.ToResponse());
    }

    [HttpGet("login-callback")]
    public async Task<IActionResult> ExternalLoginCallback(
        [FromQuery] string returnUrl,
        [FromServices] ITokenService tokenService)
    {
        var result = await _googleService.LoginUserAsync(tokenService);

        return result.IsSuccess
            ? Redirect(returnUrl)
            : BadRequest(result.ToErrors());
    }

    private string CurrentUrl => $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
}


