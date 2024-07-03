using System.Net.Http.Json;

namespace RTA.Core.WebDriver.Commands;

public class GetStatusCommand(HttpClient client)
{
    public async Task<GetStatusResponse> RunAsync()
    {
        var response = await client.GetAsync("http://localhost:9515/status");
        if (response.IsSuccessStatusCode)
        {
            var x = await response.Content.ReadFromJsonAsync<Response<GetStatusResponse>>();
            return x.Value;
        }

        throw new HttpRequestException();
    }
}