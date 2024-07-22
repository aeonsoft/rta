using System.Text.Json;

namespace RTA.Core.WebDriver.Commands;

public class GetElementAttributeCommand(
    Settings settings, HttpClient client, string sessionId, string elementId, string attribute) 
    : ICommand<string>
{
    public async Task<string?> RunAsync()
    {
        var url = $"http://localhost:{settings.Port}/session/{sessionId}/element/{elementId}/attribute/{attribute}";
        var response = await client.GetAsync(url);
        var result = JsonSerializer.Deserialize<GetElementTextResponse>(await response.Content.ReadAsStringAsync());
        return result?.Value;
    }
}