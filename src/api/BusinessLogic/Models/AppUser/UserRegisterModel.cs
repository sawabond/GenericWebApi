namespace BusinessLogic.Models.AppUser;

public sealed record UserRegisterModel
    (string UserName,
    string Password,
    string Email);
