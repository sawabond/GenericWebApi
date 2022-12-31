using BusinessLogic.Models.AppUser;
using FluentValidation;
using static BusinessLogic.Validation.AppUser.Rules;

namespace BusinessLogic.Validation.AppUser;

internal sealed class CreateValidator : AbstractValidator<CreateUserModel>
{
    public CreateValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.PhoneNumber).Matches(PhoneNumberRegex);
        RuleFor(x => x.UserName).NotEmpty().Length(3, 20);
        RuleFor(x => x.Password).Matches(PasswordRegex).WithMessage(PasswordMessage);
    }
}
