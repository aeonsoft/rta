namespace RTA.Core.WebDriver.Commands.ElementClick;

public class ElementClickCommand(Settings settings, HttpClient client, string sessionId, string elementId)
    :ICommand<bool>
{
    public async Task<bool> RunAsync()
    {
        var url = $"http://localhost:{settings.Port}/session/{sessionId}/element/{elementId}/click";
        var response = await client.PostAsync(url, new StringContent("{}"));
        return response.IsSuccessStatusCode;
    }
}