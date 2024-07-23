using System.Dynamic;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace RTA.Core.WebDriver.Commands;



public class GetActiveElementCommand(Settings settings, HttpClient client, string sessionId)
    : ICommand<string?>
{
    public async Task<string?> RunAsync()
    {
        var url = $"http://localhost:{settings.Port}/session/{sessionId}/element/active";
        var response = await client.GetAsync(url);

        var result = await response.Content.ReadFromJsonAsync<Response<JsonNode>>();
        var json = result?.Value?.AsObject();
        if (json is null || json.Count == 0)
            return null;

        return json.First().Value?.ToString();
        
    }
}