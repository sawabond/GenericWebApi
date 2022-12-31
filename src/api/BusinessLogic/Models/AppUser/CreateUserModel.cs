namespace BusinessLogic.Models.AppUser;

public sealed record CreateUserModel
    (string UserName,
    string Email,
    string Password,
    string PhoneNumber);
