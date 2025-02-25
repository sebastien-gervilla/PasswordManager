using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace PasswordManager.Core.Password;

public class PasswordHelper
{
    public static List<string> GetPasswordValidationErrors(CreatePasswordModel password)
    {
        List<string> errors = [];

        if (string.IsNullOrEmpty(password.Name))
            errors.Add("Name must not not be empty");

        if (string.IsNullOrEmpty(password.Encrypted) || !Regex.IsMatch(password.Encrypted, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{14,64}$"))
            errors.Add("Password must be between 14 and 64 characters, with at least 1 lower case letter, 1 upper case letter, and 1 digit.");

        if (string.IsNullOrEmpty(password.Category))
            errors.Add("Category must not be empty");

        return errors;
    }

    // Generation

    public const string lowercaseCharacters = "abcdefghijklmnopqrstuvwxyz";
    public const string uppercaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string numbers = "0123456789";
    public const string specialCharacters = "!@#$%^&*()_-+=<>?";

    public class PasswordGenerationOptions
    {
        public int Length { get; set; }
        public bool UseLowercaseCharacters { get; set; }
        public bool UseUppercaseCharacters { get; set; }
        public bool UseNumbers { get; set; }
        public bool UseSpecialCharacters { get; set; }
    }

    public static string GeneratePassword(PasswordGenerationOptions options)
    {
        var characters = new StringBuilder();
        if (options.UseLowercaseCharacters)
            characters.Append(lowercaseCharacters);
        if (options.UseUppercaseCharacters)
            characters.Append(uppercaseCharacters);
        if (options.UseNumbers)
            characters.Append(numbers);
        if (options.UseSpecialCharacters)
            characters.Append(specialCharacters);

        if (options.Length < 14)
            throw new ArgumentException("Password generation length must be greater than 14.");

        if (options.Length > 64)
            throw new ArgumentException("Password generation length must be lower than or equal to 64.");

        if (characters.Length == 0)
            throw new ArgumentException("No generation options selected.");

        var password = new StringBuilder(options.Length);
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] randomBytes = new byte[options.Length];

            rng.GetBytes(randomBytes);
            for (int i = 0; i < options.Length; i++)
                password.Append(characters[randomBytes[i] % characters.Length]);
        }

        return password.ToString();
    }
}
