using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;

namespace BusinessLogic.Tests.Services;

internal static class MockHelpers
{
    public static Mock<UserManager<TUser>> TestUserManager<TUser>(IUserStore<TUser> store = null) where TUser : class
    {
        store = store ?? new Mock<IUserStore<TUser>>().Object;
        var options = new Mock<IOptions<IdentityOptions>>();
        var idOptions = new IdentityOptions();
        idOptions.Lockout.AllowedForNewUsers = false;
        options.Setup(o => o.Value).Returns(idOptions);
        var userValidators = new List<IUserValidator<TUser>>();
        var validator = new Mock<IUserValidator<TUser>>();
        userValidators.Add(validator.Object);
        var pwdValidators = new List<PasswordValidator<TUser>>();
        pwdValidators.Add(new PasswordValidator<TUser>());
        var userManager = new Mock<UserManager<TUser>>(store, options.Object, new PasswordHasher<TUser>(),
            userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(), null,
            new Mock<ILogger<UserManager<TUser>>>().Object);
        validator.Setup(v => v.ValidateAsync(userManager.Object, It.IsAny<TUser>()))
            .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

        return userManager;
    }

    public static Mock<SignInManager<TUser>> TestSignInManager<TUser>() where TUser : class
    {
        return new Mock<SignInManager<TUser>>(
              TestUserManager<TUser>().Object,
              new HttpContextAccessor(),
              new Mock<IUserClaimsPrincipalFactory<TUser>>().Object,
              new Mock<IOptions<IdentityOptions>>().Object,
              new Mock<ILogger<SignInManager<TUser>>>().Object,
              new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>().Object);
    }

    public static Mock<RoleManager<TRole>> TestRoleManager<TRole>() where TRole : class
    {
        return new Mock<RoleManager<TRole>>(
            new Mock<IRoleStore<TRole>>().Object,
            null,
            null,
            null,
            null);
    }
}
