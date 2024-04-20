namespace DualJobDate.BusinessObjects.Entities;

public static class Constants
{
    public static class JwtConstants
    {
        /// <summary>
        ///     expires in 24 hours
        /// </summary>
        public static int ExpiresInMinutes = 1400;
    }

    public static string LowerCase = "abcdefghijklmnopqrstuvwxyz";
    public static string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public static string Digits = "0123456789";
    public static string SpecialChars = "!@#$%^&*()_-+=[{]};:<>|./?";
    public static int PasswordLength = 12;
}