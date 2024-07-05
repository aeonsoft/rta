namespace RTA.Core.WebDriver.Commands.ForwardCommand;

/// <summary>
/// https://www.w3.org/TR/webdriver2/#forward
/// </summary>
/// <param name="settings"></param>
/// <param name="client"></param>
/// <param name="sessionId"></param>
public class ForwardCommand(Settings settings, HttpClient client, string sessionId) : ICommand<bool>
{
    public async Task<bool> RunAsync()
    {
        var url = $"http://localhost:{settings.Port}/session/{sessionId}/forward";
        var response = await client.PostAsync(url, null);
        return response.IsSuccessStatusCode;
    }
}
