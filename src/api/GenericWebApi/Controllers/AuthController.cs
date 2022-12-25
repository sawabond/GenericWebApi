using AutoMapper;
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
}
