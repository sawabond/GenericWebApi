using AutoMapper;
using BusinessLogic.Core;
using BusinessLogic.Filtering.AppUser;
using BusinessLogic.Models.AppUser;
using BusinessLogic.Services;
using BusinessLogic.Tests.Helpers;
using BusinessLogic.Validation.Abstractions;
using DataAccess.Abstractions;
using DataAccess.Entities;
using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq.Expressions;

namespace BusinessLogic.Tests.Services;

public sealed class UserServiceTests
{
    private readonly UserService _userService;
    private readonly IMapper _mapper;
    private readonly Mock<IUserRepository> _repository;
    private readonly Mock<UserManager<AppUser>> _userManager;
    private readonly Mock<IModelValidator> _validator;

    public UserServiceTests()
	{
        _userManager = MockHelpers.TestUserManager<AppUser>();

        _userManager
            .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManager
            .Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), Roles.User))
            .ReturnsAsync(IdentityResult.Success);

        _repository = new Mock<IUserRepository>();
        _repository
            .Setup(x => x.FindAsync(
                It.IsAny<Expression<Func<AppUser, bool>>>()))
            .ReturnsAsync(EmptyUsers);


        _validator = new Mock<IModelValidator>();
        _validator.Setup(v => v.Validate(It.IsAny<It.IsAnyType>())).Returns(Result.Ok());

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TestProfile>();
        });
        _mapper = config.CreateMapper();

        _userService = new UserService(
            _repository.Object,
            _mapper,
            _userManager.Object,
            _validator.Object);
    }

    private AppUser User => new AppUser { Id = Guid.Empty.ToString(), UserName = "Test" };

    private IQueryable<AppUser> Users => new List<AppUser>
    {
        new AppUser { Id = Guid.Empty.ToString(), UserName = "A" },
        new AppUser { Id = Guid.Empty.ToString(), UserName = "B" },
        new AppUser { Id = Guid.Empty.ToString(), UserName = "C" }
    }.AsQueryable();

    private IQueryable<AppUser> EmptyUsers => new List<AppUser>().AsQueryable();

    [Fact]
	public async void GetUsersAsync_ReturnsAllUsers_IfUsersReturned()
	{
        _repository.Setup(x => x.GetAllAsync(null)).ReturnsAsync(Users);

        var result = await _userService.GetUsersAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(_mapper.Map<IEnumerable<UserViewModel>>(Users));
    }

    [Fact]
    public async void GetUsersAsync_AppliesFilter_IfFilterNotNull()
    {
        var filter = new AppUserFilter { Id = Guid.Empty.ToString() };
        var result = await _userService.GetUsersAsync(filter);

        _repository.Verify(x => x.GetAllAsync(It.Is<AppUserFilter>(y => y.Id == filter.Id)), Times.Once);
    }

    [Fact]
    public async void GetUserByIdAsync_ReturnsFail_IfUserNotFound()
    {
        _repository.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(null as AppUser);
        var id = Guid.NewGuid().ToString();

        var result = await _userService.GetUserByIdAsync(id);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainEquivalentOf(new Error(UserService.NotFound(id)));
    }

    [Fact]
    public async void GetUserByIdAsync_ReturnsOk_IfSuccessfullyFound()
    {
        var user = User;
        _repository.Setup(x => x.GetAsync(user.Id)).ReturnsAsync(user);

        var result = await _userService.GetUserByIdAsync(user.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(_mapper.Map<UserViewModel>(user));
    }

    [Fact]
    public async void PatchUserAsync_ReturnsFailWithValidationErrors_IfModelNotValid()
    {
        _validator.Setup(x => x.Validate(It.IsAny<It.IsAnyType>())).Returns(Result.Fail("Validation errors"));

        var result = await _userService.PatchUserAsync(null, new UserPatchModel(null, null, null, false));

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainEquivalentOf(new Error("Validation errors"));
    }

    [Fact]
    public async void PatchUserAsync_ReturnsFail_IfUserNotFound()
    {
        _repository.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(null as AppUser);
        var id = Guid.NewGuid().ToString();

        var result = await _userService.PatchUserAsync(id, new UserPatchModel(null, null, null, false));

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainEquivalentOf(new Error(UserService.NotFound(id)));
    }

    [Fact]
    public async void PatchUserAsync_ReturnsOk_IfFoundAndPatched()
    {
        var user = User;
        _repository.Setup(x => x.GetAsync(user.Id)).ReturnsAsync(user);
        _repository.Setup(x => x.ConfirmAsync()).ReturnsAsync(1);
        var patchModel = new UserPatchModel("Name", "Email", "Phone", true);

        var result = await _userService.PatchUserAsync(user.Id, patchModel);
        var resultUser = _mapper.Map<UserPatchModel>(user);

        result.IsSuccess.Should().BeTrue();
        _repository.Verify(x => x.ConfirmAsync(), Times.Once);
        resultUser.Should().BeEquivalentTo(patchModel);
    }

    [Fact]
    public async void PatchUserAsync_ReturnsFail_IfNotAbleToSaveChanges()
    {
        var user = User;
        _repository.Setup(x => x.GetAsync(user.Id)).ReturnsAsync(user);
        _repository.Setup(x => x.ConfirmAsync()).ReturnsAsync(0);

        var result = await _userService.PatchUserAsync(user.Id, new UserPatchModel(null, null, null, false));

        result.IsSuccess.Should().BeFalse();
        _repository.Verify(x => x.ConfirmAsync(), Times.Once);
        result.Errors.Should().ContainEquivalentOf(new Error("Unable to save changes while patching user"));
    }

    [Fact]
    public async void DeleteUserAsync_ReturnsFail_IfUserNotFound()
    {
        _repository.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(null as AppUser);
        var id = Guid.NewGuid().ToString();

        var result = await _userService.DeleteUserAsync(id);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainEquivalentOf(new Error(UserService.NotFound(id)));
    }

    [Fact]
    public async void DeleteUserAsync_ReturnsOk_IfDeletedSuccessfully()
    {
        var user = User;
        _repository.Setup(x => x.GetAsync(user.Id)).ReturnsAsync(user);
        _repository.Setup(x => x.ConfirmAsync()).ReturnsAsync(1);

        var result = await _userService.DeleteUserAsync(user.Id);

        result.IsSuccess.Should().BeTrue();
        _repository.Verify(x => x.Remove(It.Is<AppUser>(y => y.Id == user.Id)), Times.Once);
        _repository.Verify(x => x.ConfirmAsync(), Times.Once);
    }

    [Fact]
    public async void DeleteUserAsync_ReturnsFail_IfUnableToSaveChanges()
    {
        var user = User;
        _repository.Setup(x => x.GetAsync(user.Id)).ReturnsAsync(user);
        _repository.Setup(x => x.ConfirmAsync()).ReturnsAsync(0);

        var result = await _userService.DeleteUserAsync(user.Id);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainEquivalentOf(new Error("Unable to save changes while deleting user"));
        _repository.Verify(x => x.Remove(It.Is<AppUser>(y => y.Id == user.Id)), Times.Once);
        _repository.Verify(x => x.ConfirmAsync(), Times.Once);
    }

    [Fact]
    public async void CreateUserAsync_ReturnsFailWithValidationErrors_IfModelNotValid()
    {
        _validator.Setup(x => x.Validate(It.IsAny<It.IsAnyType>())).Returns(Result.Fail("Validation errors"));

        var result = await _userService.CreateUserAsync(null, null);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainEquivalentOf(new Error("Validation errors"));
    }

    [Fact]
    public async void CreateUserAsync_ReturnsFail_IfRoleNotAllowed()
    {
        var notAllowedRole = "Some role";
        var result = await _userService.CreateUserAsync(null, notAllowedRole);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainEquivalentOf(new Error($"The role {notAllowedRole} is not allowed"));
    }

    [Fact]
    public async void CreateUserAsync_ReturnsFail_IfUserExists()
    {
        _repository
            .Setup(x => x.FindAsync(
                It.IsAny<Expression<Func<AppUser, bool>>>()))
            .ReturnsAsync(Users.Take(1));

        var createModel = _mapper.Map<UserCreateModel>(User);
        var result = await _userService.CreateUserAsync(createModel, Roles.User);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainEquivalentOf(new Error("The user already exists"));
    }

    [Fact]
    public async void CreateUserAsync_ReturnsOkWithId_IfUserCreated()
    {
        var createModel = _mapper.Map<UserCreateModel>(User) with { Password = "Pa$$w0rd" };
        var result = await _userService.CreateUserAsync(createModel, Roles.User);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNullOrEmpty();
        _userManager.Verify(x => x.CreateAsync(
            It.Is<AppUser>(u => u.UserName == User.UserName), "Pa$$w0rd"), Times.Once());
        _userManager.Verify(x => x.AddToRoleAsync(
            It.Is<AppUser>(u => u.UserName == User.UserName), Roles.User), Times.Once());
    }

    [Fact]
    public async void CreateUserAsync_ReturnsFail_IfUnableToCreateUser()
    {
        _userManager
            .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());

        var createModel = _mapper.Map<UserCreateModel>(User) with { Password = "Pa$$w0rd" };
        var result = await _userService.CreateUserAsync(createModel, Roles.User);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainEquivalentOf(new Error("Unable to save changes while creating the user"));
        _userManager.Verify(x => x.CreateAsync(
            It.Is<AppUser>(u => u.UserName == User.UserName), "Pa$$w0rd"), Times.Once());
    }

    [Fact]
    public async void CreateUserAsync_ReturnsFail_IfUnableToAddToRole()
    {
        _userManager
            .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManager
            .Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), Roles.User))
            .ReturnsAsync(IdentityResult.Failed());

        var createModel = _mapper.Map<UserCreateModel>(User) with { Password = "Pa$$w0rd" };
        var result = await _userService.CreateUserAsync(createModel, Roles.User);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainEquivalentOf(new Error("Unable to save changes while creating the user"));
        _userManager.Verify(x => x.CreateAsync(
            It.Is<AppUser>(u => u.UserName == User.UserName), "Pa$$w0rd"), Times.Once());
        _userManager.Verify(x => x.AddToRoleAsync(
            It.Is<AppUser>(u => u.UserName == User.UserName), Roles.User), Times.Once());
    }
}
