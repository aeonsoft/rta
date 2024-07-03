using System.Net;
using System.Runtime.InteropServices;
using RTA.Core.Interpreters;
using RTA.Core.WebDriver;
using RTA.Core.WebDriver.Commands;
using RTA.Core.WebDriver.Commands.NewSession;

namespace RTA.Core.Tests;

public class WebDriverTests
{
    private readonly Settings _webDriverSettings = new Settings
    {
        Port = 9515
    };
    
    private readonly HttpClient _httpClient = new HttpClient();
    
     
    [Fact]
    public async Task WebDriverShouldBeRunningOnDefaultPort()
    {
        var response = await _httpClient.GetAsync("http://localhost:9515/status");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task GetStatus_ShouldReturnServerIsReady()
    {
        var command = new GetStatusCommand(_webDriverSettings, _httpClient);

        var result = await command.RunAsync();

        Assert.NotNull(result);
        Assert.True(result.Ready);
    }
   
    
    [Fact]
    public async Task NewSessionCommand_ShouldRetrieveASessionId()
    {
        var command = new NewSessionCommand(_webDriverSettings, _httpClient);

        var result = await command.RunAsync();

        Assert.NotNull(result);
        Assert.NotNull(result.SessionId);
    }
}