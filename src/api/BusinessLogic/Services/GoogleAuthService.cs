using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.Models.AppUser;
using DataAccess.Entities;
using FluentResults;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BusinessLogic.Services;

public sealed class GoogleAuthService : IGoogleAuthService
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;

    public GoogleAuthService(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IMapper mapper,
        ITokenService tokenService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    public async Task<Result<UserAuthModel>> LoginUserAsync()
    {
        var loginInfo = await _signInManager.GetExternalLoginInfoAsync();

        if (loginInfo is null)
        {
            return Result.Fail("Error while loading external login information");
        }

        var signInResult = await _signInManager.ExternalLoginSignInAsync(
            loginInfo.LoginProvider,
            loginInfo.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true);

        if (signInResult.Succeeded)
        {
            var currentEmail = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            var currentUser = await _userManager.FindByEmailAsync(currentEmail);
            return await MapUserToViewWithToken(_tokenService, currentUser);
        }

        var email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);

        if (email is null)
        {
            Result.Fail("Could not get email data");
        }

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new AppUser
            {
                UserName = loginInfo.Principal.FindFirstValue(ClaimTypes.Email),
                Email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email),
                EmailConfirmed = true,
            };

            await _userManager.CreateAsync(user);
        }

        await _userManager.AddLoginAsync(user, loginInfo);
        await _signInManager.SignInAsync(user, isPersistent: false);

        return await MapUserToViewWithToken(_tokenService, user);
    }

    public async Task<Result<AuthenticationProperties>> ConfigureExternalAuthenticationProperties(string redirectUrl) =>
        Result.Ok(_signInManager.ConfigureExternalAuthenticationProperties("google", redirectUrl));


    private async Task<Result<UserAuthModel>> MapUserToViewWithToken(ITokenService tokenService, AppUser currentUser)
    {
        var tokenResult = await tokenService.CreateTokenAsync(currentUser);

        if (tokenResult.IsFailed)
        {
            return Result.Fail(tokenResult.Errors);
        }

        return Result.Ok(_mapper.Map<UserAuthModel>(currentUser) with { Token = tokenResult.Value });
    }
}
