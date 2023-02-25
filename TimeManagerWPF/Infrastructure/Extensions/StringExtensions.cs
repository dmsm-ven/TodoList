using MahApps.Metro.IconPacks;
using System;

namespace TodoList.WPF.Infrastructure.Extensions;

public static class StringExtensions
{
    public static string GetMD5(this string input)
    {
        if (input == null)
        {
            return string.Empty;
        }
        return CreateMD5(input);
    }

    private static string CreateMD5(string input)
    {
        // Use input string to calculate MD5 hash
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes); // .NET 5 +
        }
    }

    public static PackIconFontAwesomeKind ToFontAwesomeIcon(this string s)
    {
        if (Enum.TryParse<PackIconFontAwesomeKind>(s, out var icon))
        {
            return icon;
        }
        return PackIconFontAwesomeKind.QuestionSolid;
    }
}
