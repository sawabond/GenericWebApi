using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.Models.AppUser;
using BusinessLogic.Models.Mail;
using BusinessLogic.Validation.Abstractions;
using DataAccess.Entities;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using System.Web;

namespace BusinessLogic.Services;

public sealed class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IModelValidator _validator;
    private readonly IMailService _mailService;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IMapper mapper,
        ITokenService tokenService,
        IModelValidator validator,
        IMailService mailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
        _tokenService = tokenService;
        _validator = validator;
        _mailService = mailService;
    }

    public async Task<Result<UserAuthModel>> LoginAsync(UserLoginModel model)
    {
        var validationResult = _validator.Validate(model);
        if (!validationResult.IsSuccess) return validationResult;

        var user = await _userManager.FindByNameAsync(model.UserName);

        if (user is null)
        {
            return Result.Fail($"User with username {model.UserName} was not found");
        }

        var passwordCorrect = await _userManager.CheckPasswordAsync(user, model.Password);

        if (passwordCorrect is false)
        {
            return Result.Fail("Wrong password");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);

        if (result.Succeeded is false)
        {
            return Result.Fail("Invalid login attempt");
        }

        return await CreateTokenFor(user);
    }

    public async Task<Result<UserAuthModel>> RegisterAsync(UserRegisterModel model)
    {
        var validationResult = _validator.Validate(model);
        if (!validationResult.IsSuccess) return validationResult;

        var user = _mapper.Map<AppUser>(model);

        var identityResult = await _userManager.CreateAsync(user, model.Password);

        if (identityResult.Succeeded is false)
        {
            return Result.Fail(identityResult.Errors.Select(e => e.Description));
        }

        var userViewModel = _mapper.Map<UserAuthModel>(user);

        return Result.Ok(userViewModel);
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);

        if (user is null)
        {
            return Result.Fail($"User with id {model.UserId} was not found");
        }

        var result = await _userManager.ConfirmEmailAsync(user, model.Token);

        return result.Succeeded
            ? Result.Ok()
            : Result.Fail(result.Errors.Select(e => e.Description));
    }

    public async Task<Result> SendEmailConfirmationAsync(string userId, string confirmUrl, string callbackUrl)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Result.Fail($"User with id {userId} was not found");
        }

        if (user.EmailConfirmed)
        {
            return Result.Fail("Email of user is already confirmed");
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        token = HttpUtility.UrlEncode(token);
        callbackUrl = HttpUtility.UrlEncode(callbackUrl);

        var link = $"{confirmUrl}?userId={user.Id}&token={token}&callbackUrl={callbackUrl}";

        var mail = new MailData(
            new List<string> { user.Email },
            "Email confirmation",
            $"Confirm your email by <a href={link}>this link</a>");

        var sendEmailResult = await _mailService.SendAsync(mail);

        if (sendEmailResult.IsFailed)
        {
            return Result.Fail(sendEmailResult.Errors);
        }

        return Result.Ok();
    }

    private async Task<Result<UserAuthModel>> CreateTokenFor(AppUser user)
    {
        var tokenResult = await _tokenService.CreateTokenAsync(user);

        if (tokenResult.IsFailed)
        {
            return Result.Ok(_mapper.Map<UserAuthModel>(user)).WithErrors(tokenResult.Errors);
        }

        var userViewModel = _mapper.Map<UserAuthModel>(user) with { Token = tokenResult.Value };

        return Result.Ok(userViewModel);
    }
}
