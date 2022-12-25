using DataAccess.Entities;
using FluentResults;

namespace BusinessLogic.Abstractions;

public interface ITokenService
{
    Task<Result<string>> CreateTokenAsync(AppUser user);
}
