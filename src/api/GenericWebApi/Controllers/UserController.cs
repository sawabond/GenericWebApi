using BusinessLogic.Abstractions;
using DataAccess.Entities;
using GenericWebApi.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core = Microsoft.AspNetCore.Identity;

namespace GenericWebApi.Controllers;

// TODO: Remove links to data access
[Route("api/[controller]")]
public sealed class UserController : ControllerBase
{
    private readonly Core.UserManager<AppUser> _userManager;
    private readonly IUserService _userService;

    public UserController(Core.UserManager<AppUser> userManager, IUserService userService)
    {
        _userManager = userManager;
        _userService = userService;
    }

    [HttpGet("current-user")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());

        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var usersResult = await _userService.GetUsersAsync();

        return usersResult.IsSuccess
            ? Ok(usersResult.ToResponse()) 
            : BadRequest(usersResult.ToResponse());
    }
}
