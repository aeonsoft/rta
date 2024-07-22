using System.Text.Json;
using System.Text.Json.Serialization;

namespace RTA.Core.WebDriver.Commands;

public class GetElementCssCommand(Settings settings, HttpClient client, string sessionId, string elementId, string property)
    :ICommand<GetElementCssResponse>
{
    public async Task<GetElementCssResponse?> RunAsync()
    {
        var url = $"http://localhost:{settings.Port}/session/{sessionId}/element/{elementId}/css/{property}";
        var response = await client.GetAsync(url);
        var result = JsonSerializer.Deserialize<GetElementCssResponse>(await response.Content.ReadAsStringAsync());
        return result;
    }
}

public record GetElementCssResponse
{
    [JsonPropertyName("value")]
    public string? Value { get; init; }   
}