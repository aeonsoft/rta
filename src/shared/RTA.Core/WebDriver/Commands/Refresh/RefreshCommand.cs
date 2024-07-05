namespace RTA.Core.WebDriver.Commands.RefreshCommand;

/// <summary>
/// https://www.w3.org/TR/webdriver2/#refresh
/// </summary>
/// <param name="settings"></param>
/// <param name="client"></param>
/// <param name="sessionId"></param>
public class RefreshCommand(Settings settings, HttpClient client, string sessionId): ICommand<bool>
{
    public async Task<bool> RunAsync()
    {
        var url = $"http://localhost:{settings.Port}/session/{sessionId}/back";
        var response = await client.PostAsync(url, null);
        return response.IsSuccessStatusCode;
    }
}