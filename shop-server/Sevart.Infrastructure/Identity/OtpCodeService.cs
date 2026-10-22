using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Sevart.Application.Abstractions.Identity;

namespace Sevart.Infrastructure.Identity;

public sealed class OtpCodeService
    : IOtpCodeService
{
    private readonly OtpOptions _options;
    private readonly byte[] _hashKey;

    public OtpCodeService(
        IOptions<OtpOptions> options)
    {
        _options = options.Value;

        try
        {
            _hashKey =
                Convert.FromBase64String(
                    _options.HashKey);
        }
        catch (FormatException exception)
        {
            throw new InvalidOperationException(
                "OTP hash key must be a valid Base64 value.",
                exception);
        }

        if (_hashKey.Length < 32)
        {
            throw new InvalidOperationException(
                "OTP hash key must contain at least 32 bytes.");
        }
    }

    public string GenerateCode()
    {
        var maximumValue =
            GetMaximumValue(
                _options.CodeLength);

        var value =
            RandomNumberGenerator.GetInt32(
                maximumValue);

        return value.ToString(
            $"D{_options.CodeLength}",
            CultureInfo.InvariantCulture);
    }

    public string ComputeHash(
        Guid challengeId,
        string code)
    {
        ValidateChallengeId(challengeId);

        var normalizedCode =
            NormalizeCodeDigits(code);

        ValidateCode(normalizedCode);

        var payload =
            $"{challengeId:N}:{normalizedCode}";

        using var hmac =
            new HMACSHA256(_hashKey);

        var hash =
            hmac.ComputeHash(
                Encoding.UTF8.GetBytes(payload));

        return Convert.ToHexString(hash);
    }

    public bool VerifyCode(
    Guid challengeId,
    string code,
    string expectedHash)
    {
        if (challengeId == Guid.Empty ||
            string.IsNullOrWhiteSpace(code) ||
            string.IsNullOrWhiteSpace(expectedHash))
        {
            return false;
        }

        var normalizedCode =
            NormalizeCodeDigits(code);

        if (normalizedCode.Length !=
                _options.CodeLength ||
            normalizedCode.Any(character =>
                !char.IsAsciiDigit(character)))
        {
            return false;
        }

        byte[] storedHash;

        try
        {
            storedHash =
                Convert.FromHexString(
                    expectedHash);
        }
        catch (FormatException)
        {
            return false;
        }

        var computedHash =
            Convert.FromHexString(
                ComputeHash(
                    challengeId,
                    normalizedCode));

        return CryptographicOperations.FixedTimeEquals(
            computedHash,
            storedHash);
    }

    private static int GetMaximumValue(
        int codeLength)
    {
        var maximumValue = 1;

        for (var index = 0;
             index < codeLength;
             index++)
        {
            maximumValue *= 10;
        }

        return maximumValue;
    }

    private static string NormalizeCodeDigits(
    string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return string.Empty;
        }

        var normalizedCharacters =
            code
                .Trim()
                .Select(character =>
                    character switch
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

                        _ => character
                    })
                .ToArray();

        return new string(
            normalizedCharacters);
    }

    private static void ValidateChallengeId(
        Guid challengeId)
    {
        if (challengeId == Guid.Empty)
        {
            throw new ArgumentException(
                "OTP challenge ID is required.",
                nameof(challengeId));
        }
    }

    private void ValidateCode(
        string code)
    {
        if (string.IsNullOrWhiteSpace(code) ||
            code.Length != _options.CodeLength ||
            code.Any(character =>
                !char.IsAsciiDigit(character)))
        {
            throw new ArgumentException(
                "OTP code format is invalid.",
                nameof(code));
        }
    }
}