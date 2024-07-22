using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace RTA.Core.WebDriver.Commands;


/// <summary>
/// Takes a screenshot of the top-level browsing context's viewport.
/// https://www.w3.org/TR/webdriver2/#take-screenshot
/// </summary>
/// <param name="settings"></param>
/// <param name="client"></param>
public class ScreenShotCommand(Settings settings, HttpClient client, string sessionId) 
    : ICommand<string?>
{
    public async Task<string?> RunAsync()
    {
        var url = $"http://localhost:{settings.Port}/session/{sessionId}/screenshot";
        var response = await client.GetAsync(url);
        if (response.StatusCode == HttpStatusCode.BadRequest)
            throw new NoSuchWindowException();

        var result = await response.Content.ReadFromJsonAsync<Response<string>>();
        if (result is null)
            throw new HttpRequestException("Invalid response received from web driver");

        return result.Value;
    }
    
}