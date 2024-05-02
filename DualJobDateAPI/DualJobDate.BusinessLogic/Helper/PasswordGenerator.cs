using DualJobDate.BusinessObjects.Entities.Interface.Helper;
using System.Security.Cryptography;

namespace DualJobDate.BusinessLogic.Helper;

public class PasswordGenerator : IPasswordGenerator
{
    private const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
    private const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = "0123456789";
    private const string SpecialChars = "!@#$%^&*()_-+=[{]};:<>|./?";

    [Obsolete("Obsolete")]
    public string GeneratePassword(int length = 12)
    {
        if (length < 4) throw new ArgumentException("Min 4 sign", nameof(length));

        var charCategories = new[]
        {
            LowerCase,
            UpperCase,
            Digits,
            SpecialChars
        };

        var data = new byte[length];
        using var crypto = new RNGCryptoServiceProvider();
        crypto.GetBytes(data);

        var passwordChars = new char[length];
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
            if (!passwordChars.Any(t.Contains))
            {
                var replaceIndex = random.Next(passwordChars.Length);
                passwordChars[replaceIndex] = t[random.Next(t.Length)];
            }
        }
    }
}
