using System.Dynamic;
using System.Net;
using System.Net.Http.Json;
using System.Reflection;


namespace RTA.Core.WebDriver.Commands.NewSession;

public class NewSessionCommand(Settings settings, HttpClient client) : ICommand<NewSessionResponse>
{
    public async Task<NewSessionResponse?> RunAsync()
    {
        var payload = new
        {
            capabilities = new {}
        };

        var url = $"http://localhost:{settings.Port}/session";
        var response = await client.PostAsJsonAsync(url, payload, CancellationToken.None);

        if (response.StatusCode == HttpStatusCode.InternalServerError)
            throw new HttpRequestException("Could not create a new webdriver session");

        var result = await response.Content.ReadFromJsonAsync<Response<NewSessionResponse>>();
        if (result is null)
            throw new HttpRequestException();

        return result.Value;
    }
}