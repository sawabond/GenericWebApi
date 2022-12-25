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

    public AuthService(UserManager<AppUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<Result<UserViewModel>> RegisterAsync(RegisterModel model)
    {
        var user = _mapper.Map<AppUser>(model);

        var identityResult = await _userManager.CreateAsync(user);

        return identityResult.Succeeded
            ? Result.Ok(_mapper.Map<UserViewModel>(user))
            : Result.Fail(identityResult.Errors.Select(e => e.Description));
    }
}
