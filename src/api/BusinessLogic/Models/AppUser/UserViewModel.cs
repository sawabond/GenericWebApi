namespace BusinessLogic.Models.AppUser;

public record UserViewModel
{
    public string Id { get; init; }

    public string UserName { get; init; }

    public string Email { get; init; }
}