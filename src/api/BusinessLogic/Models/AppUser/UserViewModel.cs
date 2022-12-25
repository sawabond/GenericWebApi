namespace BusinessLogic.Models.AppUser;

public sealed record UserViewModel
{
    public string UserName { get; init; }

    public string Token { get; init; }
}
