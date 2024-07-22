namespace RTA.Core.WebDriver.Commands;

public class ElementClearCommand(Settings settings, HttpClient client, string sessionId, string elementId)
    :ICommand<bool>
{
    public async Task<bool> RunAsync()
    {
        var url = $"http://localhost:{settings.Port}/session/{sessionId}/element/{elementId}/clear";
        var response = await client.GetAsync(url);
        return response.IsSuccessStatusCode;
    }
}