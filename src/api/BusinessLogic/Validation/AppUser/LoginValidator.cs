using BusinessLogic.Models.AppUser;
using FluentValidation;
using static BusinessLogic.Validation.AppUser.Rules;

namespace BusinessLogic.Validation.AppUser;

public sealed class LoginValidator : AbstractValidator<UserLoginModel>
{
	public LoginValidator()
	{
		RuleFor(x => x.UserName).NotEmpty().Length(3, 20);
		RuleFor(x => x.Password).Matches(PasswordRegex).WithMessage(PasswordMessage);
	}
}
