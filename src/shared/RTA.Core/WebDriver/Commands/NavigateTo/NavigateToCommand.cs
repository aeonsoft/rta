using System.Net;
using System.Text.Json;

namespace RTA.Core.WebDriver.Commands.NavigateTo;

public class NavigateToCommand(
    Settings settings, HttpClient client, string sessionId, string where): ICommand<bool>
{
    public async Task<bool> RunAsync()
    {
        var payload = new
        {
            url = where
        };
        
        var url = $"http://localhost:{settings.Port}/session/{sessionId}/url";
        var body = JsonSerializer.Serialize(payload);
        var response = await client.PostAsync(url, new StringContent(body));
        return response.IsSuccessStatusCode;
    }
}