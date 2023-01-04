namespace BusinessLogic.Models.AppUser;

public sealed record ConfirmEmailModel
    (string UserId,
    string Token);
