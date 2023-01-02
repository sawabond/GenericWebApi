namespace BusinessLogic.Core;

public static class Roles
{
    public const string Admin = "Admin";

    public const string Moder = "Moder";

    public const string AdminOrModer = $"{Admin},{Moder}";

    public const string User = "User";

    public static string[] AllowedRoles => new string[] { Admin, Moder, User };
}
