namespace BusinessLogic.Models.AppUser;

public sealed record UserViewModel
{
    public string Id { get; init; }

    public string UserName { get; init; }

    // TODO: Remove token to another model and split UserViewModel into ModelWithToken and ViewModel itself
    public string Token { get; init; }

    public string Email { get; init; }
}
