using System.Text.Json;
using System.Text.Json.Serialization;

namespace RTA.Core.WebDriver.Commands;

public class GetElementTextCommand(Settings settings, HttpClient client, string sessionId, string elementId) 
    : ICommand<string>
{
    public async Task<string?> RunAsync()
    {
        var url = $"http://localhost:{settings.Port}/session/{sessionId}/element/{elementId}/text";
        var response = await client.GetAsync(url);
        var result = JsonSerializer.Deserialize<GetElementTextResponse>(await response.Content.ReadAsStringAsync());
        return result?.Value;
    }
}

public record GetElementTextResponse
{
    [JsonPropertyName("value")]
    public string? Value { get; init; }   
}