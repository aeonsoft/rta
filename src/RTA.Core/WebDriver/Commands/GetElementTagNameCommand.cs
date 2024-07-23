using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace RTA.Core.WebDriver.Commands;

public class GetElementTagNameCommand
    (Settings settings, HttpClient client, string sessionId, string elementId)
    : ICommand<string?>
{
    public async Task<string?> RunAsync()
    {
        var url = $"http://localhost:{settings.Port}/session/{sessionId}/element/{elementId}/name";
        var response = await client.GetAsync(url);
        var result = await response.Content.ReadFromJsonAsync<Response<JsonNode>>();
        return result?.Value?.ToString();
    }
}