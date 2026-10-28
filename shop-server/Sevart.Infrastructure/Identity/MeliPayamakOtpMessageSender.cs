
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Sevart.Application.Abstractions.Identity;

namespace Sevart.Infrastructure.Identity;

public sealed class MeliPayamakOtpMessageSender
    : IOtpMessageSender
{
    private const string Endpoint =
        "https://rest.payamak-panel.com/api/SendSMS/BaseServiceNumber";

    private readonly HttpClient _httpClient;
    private readonly MeliPayamakOptions _options;

    public MeliPayamakOtpMessageSender(
        HttpClient httpClient,
        IOptions<MeliPayamakOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task SendSignInCodeAsync(
        string phoneNumber,
        string code,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // ملی پیامک باید شماره را در قالب داخلی دریافت کند.
        var recipient = phoneNumber;

        if (recipient.StartsWith("+98"))
        {
            recipient = "0" + recipient[3..];
        }
        else if (recipient.StartsWith("98") &&
                 recipient.Length == 12)
        {
            recipient = "0" + recipient[2..];
        }

        using var content = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                ["username"] = _options.Username,
                ["password"] = _options.Password,
                ["text"] = code,
                ["to"] = recipient,
                ["bodyId"] = _options.BodyId.ToString(
                    CultureInfo.InvariantCulture)
            });

        using var response =
            await _httpClient.PostAsync(
                Endpoint,
                content,
                cancellationToken);

        response.EnsureSuccessStatusCode();

        using var document =
            await JsonDocument.ParseAsync(
                await response.Content.ReadAsStreamAsync(
                    cancellationToken),
                cancellationToken: cancellationToken);

        var root = document.RootElement;

        if (!root.TryGetProperty("RetStatus", out var status) &&
            !root.TryGetProperty("retStatus", out status))
        {
            throw new HttpRequestException(
                "Invalid MeliPayamak response.");
        }

        if (status.ValueKind != JsonValueKind.Number ||
            !status.TryGetInt32(out var statusCode) ||
            statusCode != 1)
        {
            throw new HttpRequestException(
                $"MeliPayamak rejected the request. Status: {status}");
        }
    }
}
