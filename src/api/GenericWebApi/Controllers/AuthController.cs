using BusinessLogic.Abstractions;
using BusinessLogic.Models.AppUser;
using GenericWebApi.Extensions;
using GenericWebApi.Requests.Auth;
using Microsoft.AspNetCore.Mvc;

namespace GenericWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(new RegisterModel(request.UserName, request.Password));

        return result.IsSuccess 
            ? Ok(result.ToResponse()) 
            : BadRequest(result.ToResponse());
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(new LoginModel(request.UserName, request.Password));

        return result.IsSuccess
            ? Ok(result.ToResponse())
            : BadRequest(result.ToResponse());
    }
}
