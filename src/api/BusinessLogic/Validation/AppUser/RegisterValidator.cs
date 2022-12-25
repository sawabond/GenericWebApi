using BusinessLogic.Models.AppUser;
using FluentValidation;
using static BusinessLogic.Validation.AppUser.Rules;

namespace BusinessLogic.Validation.AppUser;

internal sealed class RegisterValidator : AbstractValidator<RegisterModel>
{
	public RegisterValidator()
	{
        RuleFor(x => x.UserName).NotEmpty().Length(3, 20);
        RuleFor(x => x.Password).NotEmpty().Length(6, 50).Matches(PasswordRegex);
    }
}
