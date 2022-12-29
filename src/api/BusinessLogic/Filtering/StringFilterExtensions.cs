namespace BusinessLogic.Filtering;

public static class StringFilterExtensions
{
    private static class Operators
    {
        public const string StartsWith = "stw";
    }
    public static string StartsWith(this string @this) =>
        $"{@this.AsCamelCase()}.{Operators.StartsWith}";

    private static string AsCamelCase(this string @this) =>
        $"{@this.ToLower()}{@this[1..]}";
}
