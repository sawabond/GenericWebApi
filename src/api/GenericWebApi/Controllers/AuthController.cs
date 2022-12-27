using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.Models.AppUser;
using GenericWebApi.Extensions;
using GenericWebApi.Requests.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.FeatureManagement.Mvc;
using BusinessLogic.FeatureManagement;

namespace GenericWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;

    public AuthController(IAuthService authService, IMapper mapper)
    {
        _authService = authService;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(_mapper.Map<RegisterModel>(request));

        return result.IsSuccess 
            ? Ok(result.ToResponse()) 
            : BadRequest(result.ToResponse());
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(_mapper.Map<LoginModel>(request));

        return result.IsSuccess
            ? Ok(result.ToResponse())
            : BadRequest(result.ToResponse());
    }

    [FeatureGate(nameof(FeatureFlags.EmailVerification))]
    [HttpPost("request-email-confirmation")]
    [Authorize]
    public async Task<IActionResult> RequestConfirmation([FromQuery] string callbackUrl)
    {
        var userId = User.Identity.GetUserId();

        var result = await _authService.SendEmailConfirmationAsync(
            userId, 
            $"{CurrentUrl}/api/auth/confirm-email",
            callbackUrl);

        return result.IsSuccess
            ? Ok()
            : BadRequest(result.ToResponse());
    }

    [FeatureGate(nameof(FeatureFlags.EmailVerification))]
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(
        [FromQuery] string userId, 
        [FromQuery] string token,
        [FromQuery] string callbackUrl)
    {
        var confirmEmailModel = new ConfirmEmailModel(userId, token);

        var result = await _authService.ConfirmEmailAsync(confirmEmailModel);

        return result.IsSuccess
            ? Redirect(callbackUrl)
            : BadRequest(result.ToResponse());
    }

    private string CurrentUrl => $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
}
