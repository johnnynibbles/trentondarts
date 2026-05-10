using Microsoft.Extensions.Options;
using System.Text.Json;

namespace TrentonDarts.Web.Services;

public class TurnstileService
{
    private readonly HttpClient _http;
    private readonly TurnstileOptions _opts;

    public TurnstileService(HttpClient http, IOptions<TurnstileOptions> opts)
    {
        _http = http;
        _opts = opts.Value;
    }

    public async Task<bool> ValidateAsync(string? token)
    {
        if (string.IsNullOrWhiteSpace(_opts.SecretKey))
            return true; // CAPTCHA not configured — allow in development

        if (string.IsNullOrWhiteSpace(token))
            return false;

        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["secret"] = _opts.SecretKey,
            ["response"] = token
        });

        var response = await _http.PostAsync("https://challenges.cloudflare.com/turnstile/v0/siteverify", form);
        if (!response.IsSuccessStatusCode)
            return false;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.TryGetProperty("success", out var success) && success.GetBoolean();
    }
}
