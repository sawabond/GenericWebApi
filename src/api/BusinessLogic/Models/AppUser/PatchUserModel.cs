namespace BusinessLogic.Models.AppUser;

public sealed record PatchUserModel
    (string UserName,
    string Email,
    string PhoneNumber,
    bool EmailConfirmed);
