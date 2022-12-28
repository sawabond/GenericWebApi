using AutoMapper;
using BusinessLogic.Abstractions;
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

    public async Task<Result<IEnumerable<UserViewModel>>> GetUsersAsync()
    {
        var users = await _repository.GetAllAsync();

        var viewModels = _mapper.Map<IEnumerable<UserViewModel>>(users);

        return Result.Ok(viewModels);
    }
}
