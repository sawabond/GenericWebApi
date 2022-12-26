using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.Models.AppUser;
using BusinessLogic.Validation.Abstractions;
using DataAccess.Entities;
using FluentResults;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Services;

public sealed class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IModelValidator _validator;

    public AuthService(
        UserManager<AppUser> userManager, 
        IMapper mapper,
        ITokenService tokenService,
        IModelValidator validator)
    {
        _userManager = userManager;
        _mapper = mapper;
        _tokenService = tokenService;
        _validator = validator;
    }

    public async Task<Result<UserViewModel>> LoginAsync(LoginModel model)
    {
        var validationResult = _validator.Validate(model);
        if (!validationResult.IsSuccess) return validationResult;

        var user = await _userManager.FindByNameAsync(model.UserName);

        if (user is null)
        {
            return Result.Fail($"User with username {model.UserName} was not found");
        }

        var isPassowordCorrect = await _userManager.CheckPasswordAsync(user, model.Password);

        if (isPassowordCorrect == false)
        {
            return Result.Fail("Wrong password");
        }

        return await CreateTokenFor(user);
    }

    public async Task<Result<UserViewModel>> RegisterAsync(RegisterModel model)
    {
        var validationResult = _validator.Validate(model);
        if (!validationResult.IsSuccess) return validationResult;

        var user = _mapper.Map<AppUser>(model);

        var identityResult = await _userManager.CreateAsync(user, model.Password);

        if (identityResult.Succeeded == false)
        {
            return Result.Fail(identityResult.Errors.Select(e => e.Description));
        }

        return await CreateTokenFor(user);
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailModel model)
    {

    }

    public async Task<Result> SendEmailConfirmationAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Result.Fail($"User with id {userId} was not found");
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    private async Task<Result<UserViewModel>> CreateTokenFor(AppUser user)
    {
        var tokenResult = await _tokenService.CreateTokenAsync(user);

        if (tokenResult.IsFailed)
        {
            return Result.Ok(_mapper.Map<UserViewModel>(user)).WithErrors(tokenResult.Errors);
        }

        var userViewModel = _mapper.Map<UserViewModel>(user) with { Token = tokenResult.Value };

        return Result.Ok(userViewModel);
    }
}
