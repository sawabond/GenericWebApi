namespace BusinessLogic.Models.AppUser;

public sealed record UserPatchModel
    (string UserName,
    string Email,
    string PhoneNumber,
    bool EmailConfirmed);
