using DataAccess.Entities;
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
