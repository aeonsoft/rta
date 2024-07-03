using System.Dynamic;
using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;


namespace RTA.Core.WebDriver.Commands.NewSession;

/// <summary>
/// https://www.w3.org/TR/webdriver2/#new-session
/// </summary>
/// <param name="settings"></param>
/// <param name="client"></param>
public class NewSessionCommand(Settings settings, HttpClient client) : ICommand<NewSessionResponse>
{
    public async Task<NewSessionResponse?> RunAsync()
    {
        var payload = new
        {
            capabilities = new { }
        };

        var url = $"http://localhost:{settings.Port}/session";
        var body = JsonSerializer.Serialize(payload);
        var response = await client.PostAsync(url, new StringContent(body));
        if (response.StatusCode == HttpStatusCode.InternalServerError)
            throw new HttpRequestException("Could not create a new webdriver session");

        var result = await response.Content.ReadFromJsonAsync<Response<NewSessionResponse>>();
        if (result is null)
            throw new HttpRequestException("Invalid response received from web driver");
        
        return result.Value;
    }
}