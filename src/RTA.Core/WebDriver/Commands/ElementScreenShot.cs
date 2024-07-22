using System.Net;
using System.Net.Http.Json;

namespace RTA.Core.WebDriver.Commands;


/// <summary>
/// Takes a screenshot of the visible region encompassed by the bounding rectangle of an element.
/// https://www.w3.org/TR/webdriver2/#take-element-screenshot
/// </summary>
/// <param name="settings"></param>
/// <param name="client"></param>
/// <param name="sessionId"></param>
/// <param name="elementId"></param>
public class ElementScreenShot(Settings settings, HttpClient client, string sessionId, string elementId) 
    : ICommand<string?>
{
    public async Task<string?> RunAsync()
    {
        var url = $"http://localhost:{settings.Port}/session/{sessionId}/element/{elementId}/screenshot";
        var response = await client.GetAsync(url);
        switch (response.StatusCode)
        {
            case HttpStatusCode.BadRequest: throw new NoSuchWindowException();
            case HttpStatusCode.NotFound: throw new NoSuchElementException();
        }
        
        var result = await response.Content.ReadFromJsonAsync<Response<string>>();
        if (result is null)
            throw new HttpRequestException("Invalid response received from web driver");

        return result.Value;
    }
}