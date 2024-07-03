using System.Net;
using System.Runtime.InteropServices;
using RTA.Core.Interpreters;
using RTA.Core.WebDriver;
using RTA.Core.WebDriver.Commands;

namespace RTA.Core.Tests;

public class WebDriverTests
{
    private readonly Settings _webDriverSettings = new Settings
    {
        Port = 9515
    };
    
    [Fact]
    public async Task WebDriverShouldBeRunningOnDefaultPort()
    {
        var client = new HttpClient();
        var response = await client.GetAsync("http://localhost:9515/status");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetStatus_ShouldReturnServerIsReady()
    {
        var client = new HttpClient();
        var command = new GetStatusCommand(_webDriverSettings, client);

        var result = await command.RunAsync();

        Assert.NotNull(result);
        Assert.True(result.Ready);
    }
    
}