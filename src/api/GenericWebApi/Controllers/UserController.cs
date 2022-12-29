using BusinessLogic.Abstractions;
using BusinessLogic.Models.AppUser;
using GenericWebApi.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenericWebApi.Controllers;

[Route("api/[controller]")]
public sealed class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("current")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userResult = await _userService.GetUserById(User.Identity.GetUserId());

        return userResult.IsSuccess
            ? Ok(userResult.ToResponse()) 
            : BadRequest(userResult.ToResponse());
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers([FromQuery] AppUserFilter filter)
    {
        var usersResult = await _userService.GetUsersAsync(filter);

        return usersResult.IsSuccess
            ? Ok(usersResult.ToResponse(filter)) 
            : BadRequest(usersResult.ToResponse(filter));
    }
}
