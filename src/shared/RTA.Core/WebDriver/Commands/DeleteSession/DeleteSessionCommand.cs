namespace RTA.Core.WebDriver.Commands.DeleteSession;

/// <summary>
/// https://www.w3.org/TR/webdriver2/#delete-session
/// </summary>
/// <param name="settings"></param>
/// <param name="client"></param>
/// <param name="sessionId"></param>
public class DeleteSessionCommand(Settings settings, HttpClient client, string sessionId) : ICommand<bool>
{
    public async Task<bool> RunAsync()
    {
        var url = $"http://localhost:{settings.Port}/session/{sessionId}";
        var response = await client.DeleteAsync(url);
        return response.IsSuccessStatusCode;
    }
}