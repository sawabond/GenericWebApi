using BusinessLogic.Abstractions;
using BusinessLogic.Filtering.AppUser;
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

        return userResult.ToObjectResponse();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers([FromQuery] AppUserFilter filter)
    {
        var usersResult = await _userService.GetUsersAsync(filter);

        return usersResult.ToObjectResponse(filter);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser()
    {
        throw new NotImplementedException();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser()
    {
        throw new NotImplementedException();
    }

    [HttpPatch]
    public async Task<IActionResult> PatchUser()
    {
        throw new NotImplementedException();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser()
    {
        throw new NotImplementedException();
    }
}
