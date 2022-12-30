using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.Filtering.AppUser;
using BusinessLogic.Models.AppUser;
using DataAccess.Abstractions;
using FluentResults;

namespace BusinessLogic.Services;

internal sealed class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result> DeleteUserAsync(string id)
    {
        var user = await _repository.GetAsync(id);

        if (user is null)
        {
            return Result.Fail($"User with id {id} was not found");
        }

        _repository.Remove(user);
        return await _repository.ConfirmAsync() > 0
            ? Result.Ok()
            : Result.Fail("Unable to save changes while deleting user");
    }

    public async Task<Result<UserViewModel>> GetUserByIdAsync(string id)
    {
        var user = await _repository.GetAsync(id);

        if (user is null)
        {
            return Result.Fail($"User with id {id} was not found");
        }

        var viewModel = _mapper.Map<UserViewModel>(user);

        return Result.Ok(viewModel);
    }

    public async Task<Result<IEnumerable<UserViewModel>>> GetUsersAsync(AppUserFilter filter = null)
    {
        var users = await _repository.GetAllAsync(filter);

        var viewModels = _mapper.Map<IEnumerable<UserViewModel>>(users);

        return Result.Ok(viewModels);
    }

    public async Task<Result> PatchUserAsync(string id, PatchUserModel model)
    {
        var user = await _repository.GetAsync(id);

        if (user is null)
        {
            return Result.Fail($"User with id {id} was not found");
        }

        _mapper.Map(model, user);

        return await _repository.ConfirmAsync() > 0
            ? Result.Ok()
            : Result.Fail("Unable to save changes while patching user");
    }
}
