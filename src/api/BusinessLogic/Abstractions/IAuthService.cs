using BusinessLogic.Models.AppUser;
using FluentResults;

namespace BusinessLogic.Abstractions;

public interface IAuthService
{
    Task<Result<UserAuthModel>> RegisterAsync(UserRegisterModel model);

    Task<Result<UserAuthModel>> LoginAsync(UserLoginModel model);

    Task<Result> ConfirmEmailAsync(ConfirmEmailModel model);

    Task<Result> SendEmailConfirmationAsync(string userId, string confirmUrl, string callbackUrl);
}
