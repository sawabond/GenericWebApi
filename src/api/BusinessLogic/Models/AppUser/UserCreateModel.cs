namespace BusinessLogic.Models.AppUser;

public sealed record UserCreateModel
    (string UserName,
    string Email,
    string Password,
    string PhoneNumber);
