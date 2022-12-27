using DataAccess.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Core = Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GenericWebApi.Controllers;

[Route("api/[controller]")]
public sealed class UserController : ControllerBase
{
    private readonly Core.UserManager<AppUser> _userManager;

    public UserController(Core.UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("current-user")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());

        return Ok(user);
    }
}
