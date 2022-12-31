using BusinessLogic.Abstractions;
using BusinessLogic.Core;
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
    [Authorize(Roles = Roles.User)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userResult = await _userService.GetUserByIdAsync(User.Identity.GetUserId());

        return userResult.ToObjectResponse();
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = Roles.User)]
    public async Task<IActionResult> GetUserById(string id)
    {
        var userResult = await _userService.GetUserByIdAsync(id);

        return userResult.ToObjectResponse();
    }

    [HttpGet]
    [Authorize(Roles = Roles.AdminOrModer)]
    public async Task<IActionResult> GetAllUsers([FromQuery] AppUserFilter filter)
    {
        var usersResult = await _userService.GetUsersAsync(filter);

        return usersResult.ToObjectResponse(filter);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserModel model, [FromQuery] string role)
    {
        var createUserResult = await _userService.CreateUserAsync(model, role);

        return createUserResult.ToObjectResponse();
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> PatchUser(string id, [FromBody] PatchUserModel model)
    {
        var patchUserResult = await _userService.PatchUserAsync(id, model);

        return patchUserResult.ToNoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var deleteUserResult = await _userService.DeleteUserAsync(id);

        return deleteUserResult.ToNoContent();
    }
}
