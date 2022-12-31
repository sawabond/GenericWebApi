using BusinessLogic.Abstractions;
using BusinessLogic.Filtering.AppUser;
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
        var userResult = await _userService.GetUserByIdAsync(User.Identity.GetUserId());

        return userResult.ToObjectResponse();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers([FromQuery] AppUserFilter filter)
    {
        var usersResult = await _userService.GetUsersAsync(filter);

        return usersResult.ToObjectResponse(filter);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserModel model)
    {
        var createUserResult = await _userService.CreateUserAsync(model);

        return createUserResult.ToObjectResponse();
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchUser(string id, [FromBody] PatchUserModel model)
    {
        var patchUserResult = await _userService.PatchUserAsync(id, model);

        return patchUserResult.ToNoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var deleteUserResult = await _userService.DeleteUserAsync(id);

        return deleteUserResult.ToNoContent();
    }
}
