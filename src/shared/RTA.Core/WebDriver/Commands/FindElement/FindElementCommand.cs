using System.Net.Http.Json;
using System.Text.Json;
using System.Windows.Input;

namespace RTA.Core.WebDriver.Commands.FindElement;

/// <summary>
/// https://www.w3.org/TR/webdriver2/#find-element
/// Gets a value by cs id selector and return the internal webdriver id for it.
/// If the element cannot be found, rettuns null
/// </summary>
public class FindElementCommand(Settings settings, HttpClient client, string sessionId, string elementId) 
    : ICommand<string>
    
{
    public async Task<string?> RunAsync()
    {
        var payload = new
        {
            @using = "css selector",
            value =  elementId
        };
        var url = $"http://localhost:{settings.Port}/session/{sessionId}/element";
        var body = JsonSerializer.Serialize(payload);
        var response = await client.PostAsync(url, new StringContent(body));
        if (!response.IsSuccessStatusCode)
            return null;
        
        var text = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(text);
        var value = json.RootElement.GetProperty("value");
        var result = value.ToString();
        if (!result.Contains(':'))
            return null;

        result = result.Split(':')[1];
        result = result.Substring(result.IndexOf('"')+1, result.LastIndexOf('"')-1);
        return result;
    }
}