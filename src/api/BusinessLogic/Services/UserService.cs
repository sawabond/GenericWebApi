﻿using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.Filtering.AppUser;
using BusinessLogic.Models.AppUser;
using BusinessLogic.Validation.Abstractions;
using DataAccess.Abstractions;
using DataAccess.Entities;
using FluentResults;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Services;

internal sealed class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    private readonly IModelValidator _validator;

    public UserService(
        IUserRepository repository, 
        IMapper mapper, 
        UserManager<AppUser> userManager,
        IModelValidator validator)
    {
        _repository = repository;
        _mapper = mapper;
        _userManager = userManager;
        _validator = validator;
    }

    public async Task<Result<string>> CreateUserAsync(CreateUserModel model)
    {
        var validationResult = _validator.Validate(model);
        if (!validationResult.IsSuccess) return validationResult;

        var user = _mapper.Map<AppUser>(model);

        var existingUser = (await _repository
            .FindAsync(u => u.UserName == model.UserName 
            || u.Email == model.Email
            || u.PhoneNumber == model.PhoneNumber)).FirstOrDefault();

        if (existingUser is not null)
        {
            return Result.Fail($"The user already exists");
        }

        return (await _userManager.CreateAsync(user, model.Password)).Succeeded
            ? Result.Ok(user.Id)
            : Result.Fail("Unable to save changes while creating the user");
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
        var validationResult = _validator.Validate(model);
        if (!validationResult.IsSuccess) return validationResult;

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
