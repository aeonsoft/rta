using System.Text.Json;
using RTA.Core.WebDriver.Commands.GetElementText;

namespace RTA.Core.WebDriver.Commands.GetElementAttribute;

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