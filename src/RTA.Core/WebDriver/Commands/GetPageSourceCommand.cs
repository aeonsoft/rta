using System.Net;
using System.Net.Http.Json;

namespace RTA.Core.WebDriver.Commands;

/// <summary>
/// Gets a string serialization of the DOM of the current browsing context active document
/// https://www.w3.org/TR/webdriver2/#get-page-source
/// </summary>
/// <param name="settings"></param>
/// <param name="client"></param>
/// <param name="sessionId"></param>
public class GetPageSourceCommand(Settings settings, HttpClient client, string sessionId) 
    : ICommand<string?>
{
    public async Task<string?> RunAsync()
    {
        var url = $"http://localhost:{settings.Port}/session/{sessionId}/source";
        var response = await client.GetAsync(url);
        
        if (!response.IsSuccessStatusCode) 
            throw new HttpRequestException();
        
        var result  = await response.Content.ReadFromJsonAsync<Response<string?>>();
        if (result is null)
            throw new HttpRequestException();
        
        return result.Value;
    }
}