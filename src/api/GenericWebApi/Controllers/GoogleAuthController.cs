using BusinessLogic.Abstractions;
using BusinessLogic.FeatureManagement;
using GenericWebApi.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace GenericWebApi.Controllers;

[FeatureGate(nameof(FeatureFlags.GoogleAuthentication))]
[Route("api/[controller]")]
public sealed class GoogleAuthController : ControllerBase
{
    private readonly IGoogleAuthService _googleService;

    public GoogleAuthController(IGoogleAuthService googleService)
    {
        _googleService = googleService;
    }

    [HttpGet("login")]
    public async Task<IActionResult> Login([FromQuery] string returnUrl)
    {
        var redirectUrl = Url.Action(nameof(LoginCallback), "GoogleAuth", new { returnUrl });
        var propertiesResult = await _googleService.ConfigureExternalAuthenticationProperties(redirectUrl);

        return propertiesResult.IsSuccess
            ? Challenge(propertiesResult.Value, "google")
            : BadRequest(propertiesResult.ToResponse());
    }

    [HttpGet("login-callback")]
    public async Task<IActionResult> LoginCallback([FromQuery] string returnUrl)
    {
        var result = await _googleService.LoginUserAsync();

        return result.IsSuccess
            ? Redirect($"{returnUrl}?accessToken={result.Value.Token}")
            : BadRequest(result.ToErrors());
    }
}