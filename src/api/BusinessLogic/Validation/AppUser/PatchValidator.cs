using BusinessLogic.Models.AppUser;
using FluentValidation;
using static BusinessLogic.Validation.AppUser.Rules;

namespace BusinessLogic.Validation.AppUser;

internal sealed class PatchValidator : AbstractValidator<UserPatchModel>
{
    public PatchValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.PhoneNumber).Matches(PhoneNumberRegex);
        RuleFor(x => x.UserName).Length(3, 20);
    }
}
