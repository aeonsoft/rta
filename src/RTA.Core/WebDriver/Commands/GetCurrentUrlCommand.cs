using System.Text.Json;
using System.Text.Json.Serialization;

namespace RTA.Core.WebDriver.Commands;

/// <summary>
/// https://www.w3.org/TR/webdriver2/#get-current-url
/// </summary>
/// <param name="settings"></param>
/// <param name="client"></param>
/// <param name="sessionId"></param>
public class GetCurrentUrlCommand(Settings settings, HttpClient client, string sessionId) 
    : ICommand<string>
{
    public async Task<string?> RunAsync()
    {
        var url = $"http://localhost:{settings.Port}/session/{sessionId}/url";
        var response = await client.GetAsync(url);
        var result = JsonSerializer.Deserialize<GetCurrentUrlResponse>(await response.Content.ReadAsStringAsync());
        return result?.Value;
    }
}

public record GetCurrentUrlResponse
{
    [JsonPropertyName("value")]
    public string? Value { get; init; }
}