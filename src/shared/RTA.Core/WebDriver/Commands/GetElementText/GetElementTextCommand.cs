using System.Text.Json;

namespace RTA.Core.WebDriver.Commands.GetElementText;

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