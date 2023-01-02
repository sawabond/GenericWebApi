using BusinessLogic.Models.AppUser;
using FluentResults;
using Microsoft.AspNetCore.Authentication;

namespace BusinessLogic.Abstractions;

public interface IGoogleAuthService
{
    Task<Result<UserAuthModel>> LoginUserAsync();

    Task<Result<AuthenticationProperties>> ConfigureExternalAuthenticationProperties(string redirectUrl);
}
