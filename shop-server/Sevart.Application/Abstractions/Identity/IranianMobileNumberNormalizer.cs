namespace Sevart.Application.Abstractions.Identity;

public static class IranianMobileNumberNormalizer
{
    public static bool TryNormalize(
        string? value,
        out string normalizedPhoneNumber)
    {
        normalizedPhoneNumber = string.Empty;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var digits = NormalizeDigits(value);

        if (digits.StartsWith(
                "0098",
                StringComparison.Ordinal))
        {
            digits = digits[2..];
        }

        if (digits.StartsWith(
                "09",
                StringComparison.Ordinal) &&
            digits.Length == 11)
        {
            digits = $"98{digits[1..]}";
        }

        if (!digits.StartsWith(
                "989",
                StringComparison.Ordinal) ||
            digits.Length != 12 ||
            digits.Any(
                character =>
                    !char.IsAsciiDigit(character)))
        {
            return false;
        }

        normalizedPhoneNumber = $"+{digits}";

        return true;
    }

    private static string NormalizeDigits(
        string value)
    {
        var characters =
            value
                .Trim()
                .Where(
                    character =>
                        character is not ' ' and
                        not '-' and
                        not '(' and
                        not ')')
                .Select(
                    character => character switch
                    {
                        >= '\u06F0' and <= '\u06F9' =>
                            (char)(
                                '0' +
                                character -
                                '\u06F0'),

                        >= '\u0660' and <= '\u0669' =>
                            (char)(
                                '0' +
                                character -
                                '\u0660'),

                        '+' => '+',

                        _ => character
                    })
                .ToArray();

        var normalized =
            new string(characters);

        return normalized.StartsWith('+')
            ? normalized[1..]
            : normalized;
    }
}