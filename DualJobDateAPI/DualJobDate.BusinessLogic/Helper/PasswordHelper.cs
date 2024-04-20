using System.Security.Cryptography;
using DualJobDate.BusinessObjects.Entities;

namespace DualJobDate.BusinessLogic.Helper;
public static class PasswordHelper
{

    [Obsolete("Obsolete")]
    public static string GeneratePassword()
    {
        var length = Constants.PasswordLength;
        var data = new byte[length];
        var passwordChars = new char[length];

        var charCategories = new[]
        {
            Constants.LowerCase,
            Constants.UpperCase,
            Constants.Digits,
            Constants.SpecialChars
        };

        using var crypto = new RNGCryptoServiceProvider();
        crypto.GetBytes(data);

        for (var i = 0; i < length; i++)
        {
            var categoryIndex = data[i] % charCategories.Length;
            var charIndex = data[i] % charCategories[categoryIndex].Length;
            passwordChars[i] = charCategories[categoryIndex][charIndex];
        }

        EnsureEachCategory(passwordChars, charCategories);
        return new string(passwordChars);
    }

    private static void EnsureEachCategory(char[] passwordChars, string[] charCategories)
    {
        var random = new Random();
        foreach (var t in charCategories)
        {
            if (!passwordChars.Any(p => t.Contains(p)))
            {
                var replaceIndex = random.Next(passwordChars.Length);
                passwordChars[replaceIndex] = t[random.Next(t.Length)];
            }
        }
    }
}

