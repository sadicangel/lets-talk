using System.Security.Cryptography;

namespace LetsTalk;

internal static class PasswordGenerator
{
    private const string Lower = "abcdefghijklmnopqrstuvwxyz";
    private const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = "0123456789";
    private const string Special = "!@$?_-";
    private const string All = Lower + Upper + Digits + Special;

    public static string Generate(int length)
    {
        if (length < 4)
            throw new ArgumentOutOfRangeException(nameof(length), length, "Length must be at least 4");

        return string.Create(
            length,
            0,
            (chars, _) =>
            {
                RandomNumberGenerator.GetItems(Lower, chars[0..1]);
                RandomNumberGenerator.GetItems(Upper, chars[1..2]);
                RandomNumberGenerator.GetItems(Digits, chars[2..3]);
                RandomNumberGenerator.GetItems(Special, chars[3..4]);
                RandomNumberGenerator.GetItems(All, chars[4..]);
                RandomNumberGenerator.Shuffle(chars);
            });
    }
}
