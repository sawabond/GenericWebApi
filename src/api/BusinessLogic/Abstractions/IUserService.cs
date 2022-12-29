using BusinessLogic.Filtering.AppUser;
using BusinessLogic.Models.AppUser;
using FluentResults;

namespace BusinessLogic.Abstractions;

public interface IUserService
{
    Task<Result<IEnumerable<UserViewModel>>> GetUsersAsync(AppUserFilter filter);

    Task<Result<UserViewModel>> GetUserById(string id);

}
