namespace BusinessLogic.Models.AppUser;

public sealed record UserAuthModel : UserViewModel
{
    public string Token { get; init; }
}
