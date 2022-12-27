using BusinessLogic.Models.AppUser;
using FluentResults;
using Microsoft.AspNetCore.Authentication;

namespace BusinessLogic.Abstractions;

public interface IGoogleAuthService
{
    Task<Result<UserViewModel>> LoginUserAsync();

    Task<Result<AuthenticationProperties>> ConfigureExternalAuthenticationProperties(string redirectUrl);
}
