using System.Text.Json;

namespace RTA.Core.WebDriver.Commands.GetCurrentUrl;

/// <summary>
/// https://www.w3.org/TR/webdriver2/#get-current-url
/// </summary>
/// <param name="settings"></param>
/// <param name="client"></param>
/// <param name="sessionId"></param>
public class GetCurrentUrlCommand(Settings settings, HttpClient client, string sessionId) 
    : ICommand<GetCurrentUrlResponse>
{
    public async Task<GetCurrentUrlResponse?> RunAsync()
    {
        var url = $"http://localhost:{settings.Port}/session/{sessionId}/url";
        var response = await client.GetAsync(url);
        var result = JsonSerializer.Deserialize<GetCurrentUrlResponse>(await response.Content.ReadAsStringAsync());
        return result;
    }
}