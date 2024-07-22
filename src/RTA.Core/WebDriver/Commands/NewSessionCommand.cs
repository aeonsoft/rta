using System.Net;
using System.Net.Http.Json;
using System.Text.Json;


namespace RTA.Core.WebDriver.Commands;

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


public record NewSessionResponse
{ 
    public string? SessionId { get; init; }
    public SessionCapabilities? Capabilities { get; init; }
}

public record SessionCapabilities
{
    public bool AcceptInsecureCerts { get; init; }
    public string? BrowserName { get; init; }
    public string? BrowserVersion { get; init; }
    public bool? NetworkConnectionEnabled { get; init; }
    public string? PageLoadStrategy { get; init; }
    public string? PlatformName { get; init; }
    public SessionTimeouts? Timeouts { get; init; }
}

public record SessionTimeouts {
    public uint? Implicit { get; init; }
    public uint? PageLoad{ get; init; }
    public uint? Script{ get; init; }
}