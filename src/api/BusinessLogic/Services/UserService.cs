﻿using AutoMapper;
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

    public async Task<Result<UserViewModel>> GetUserById(string id)
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
}