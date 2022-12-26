using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.Models.AppUser;
using GenericWebApi.Extensions;
using GenericWebApi.Requests.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNet.Identity;

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

    [HttpPost("request-email-confirmation")]
    [Authorize]
    public async Task<IActionResult> RequestConfirmation()
    {
        var userId = User.Identity.GetUserId();

        var result = await _authService.SendConfirmation(userId);
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
    {
        var confirmEmailModel = new ConfirmEmailModel(userId, token);

       var result = await _authService.ConfirmEmail(confirmEmailModel);
    }
}
