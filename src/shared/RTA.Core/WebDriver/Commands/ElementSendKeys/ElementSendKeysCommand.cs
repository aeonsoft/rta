using System.Text.Json;

namespace RTA.Core.WebDriver.Commands.ElementSendKeys;

public class ElementSendKeysCommand(Settings settings, HttpClient client, string sessionId, string elementId, string text)
    :ICommand<bool>
{
    public async Task<bool> RunAsync()
    {
        var payload = new
        {
            text = text
        };
        
        var url = $"http://localhost:{settings.Port}/session/{sessionId}/element/{elementId}/value";
        var body = JsonSerializer.Serialize(payload);
        var response = await client.PostAsync(url, new StringContent(body));
        return response.IsSuccessStatusCode;
    }
}