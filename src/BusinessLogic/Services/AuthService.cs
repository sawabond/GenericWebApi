using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.Models.AppUser;
using DataAccess.Entities;
using FluentResults;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Services;

public sealed class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;

    public AuthService(
        UserManager<AppUser> userManager, 
        IMapper mapper,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    public async Task<Result<UserViewModel>> RegisterAsync(RegisterModel model)
    {
        var user = _mapper.Map<AppUser>(model);

        var identityResult = await _userManager.CreateAsync(user);

        if (identityResult.Succeeded == false)
        {
            return Result.Fail(identityResult.Errors.Select(e => e.Description));
        }

        var tokenResult = await _tokenService.CreateTokenAsync(user);

        if (tokenResult.IsFailed)
        {
            return Result.Ok(_mapper.Map<UserViewModel>(user)).WithErrors(tokenResult.Errors);
        }

        var userViewModel = _mapper.Map<UserViewModel>(user) with { Token = tokenResult.Value };

        return Result.Ok(userViewModel);
    }
}
