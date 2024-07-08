using System.Text.Json;

namespace RTA.Core.WebDriver.Commands.GetElementCss;

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