namespace RTA.Core.WebDriver.Commands;

/// <summary>
/// https://www.w3.org/TR/webdriver2/#back
/// </summary>
public class BackCommand(Settings settings, HttpClient client, string sessionId): ICommand<bool>
{
    public async Task<bool> RunAsync()
    {
        var url = $"http://localhost:{settings.Port}/session/{sessionId}/back";
        var response = await client.PostAsync(url, null);
        return response.IsSuccessStatusCode;
    }
}