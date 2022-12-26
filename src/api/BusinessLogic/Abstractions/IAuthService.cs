using BusinessLogic.Models.AppUser;
using FluentResults;

namespace BusinessLogic.Abstractions;

public interface IAuthService
{
    Task<Result<UserViewModel>> RegisterAsync(RegisterModel model);

    Task<Result<UserViewModel>> LoginAsync(LoginModel model);

    Task<Result> ConfirmEmailAsync(ConfirmEmailModel model);

    Task<Result> SendEmailConfirmationAsync(string userId);
}
