﻿using System.Text.Json;

namespace RTA.Core.WebDriver.Commands.GetTitle;

/// <summary>
/// https://www.w3.org/TR/webdriver2/#get-title
/// </summary>
/// <param name="settings"></param>
/// <param name="client"></param>
/// <param name="sessionId"></param>
public class GetTitleCommand(Settings settings, HttpClient client, string sessionId): ICommand<GetTitleResponse>
{
    public async Task<GetTitleResponse?> RunAsync()
    {
        var url = $"http://localhost:{settings.Port}/session/{sessionId}/title";
        var response = await client.GetAsync(url);
        var result = JsonSerializer.Deserialize<GetTitleResponse>(await response.Content.ReadAsStringAsync());
        return result;
    }
}