using System.Net.Http.Json;

namespace RTA.Core.WebDriver.Commands;

public class GetStatusCommand(Settings settings, HttpClient client) : ICommand<GetStatusResponse>
{
    public async Task<GetStatusResponse?> RunAsync()
    {
        var response = await client.GetAsync($"http://localhost:{settings.Port}/status");
        if (!response.IsSuccessStatusCode) 
            throw new HttpRequestException();
        
        var x = await response.Content.ReadFromJsonAsync<Response<GetStatusResponse>>();
        if (x is null)
            throw new HttpRequestException();
        
        return x.Value;
    }
}