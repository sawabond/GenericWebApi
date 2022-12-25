using System.Text;

namespace BusinessLogic.Extensions;

public static class StringExtensions
{
    public static byte[] ToByteArray(this string @this) =>
        Encoding.UTF8.GetBytes(@this);
}
