using BusinessLogic.Models.AppUser;
using FluentValidation;
using static BusinessLogic.Validation.AppUser.Rules;

namespace BusinessLogic.Validation.AppUser;

public sealed class RegisterValidator : AbstractValidator<UserRegisterModel>
{
	public RegisterValidator()
	{
        RuleFor(x => x.UserName).NotEmpty().Length(3, 20);
        RuleFor(x => x.Password).Matches(PasswordRegex).WithMessage(PasswordMessage);
        RuleFor(x => x.Email).EmailAddress().Length(3, 100);
    }
}
