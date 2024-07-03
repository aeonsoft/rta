using System.Net;
using System.Runtime.InteropServices;
using RTA.Core.Interpreters;
using RTA.Core.WebDriver.Commands;

namespace RTA.Core.Tests;

public class WebDriverTests
{
    [Fact]
    public async Task WebDriverShouldBeRunningOnDefaultPort()
    {
        var client = new HttpClient();
        var response = await client.GetAsync("http://localhost:9515/status");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetStatus_ShouldReturnOk()
    {
        var client = new HttpClient();
        var command = new GetStatusCommand(client);

        var result = await command.RunAsync();
        
        Assert.True(result.Ready);
    }
    
    /*
    [Fact]
    public void StartWebDriverServer_ShouldSpawnNewProcess()
    {
        //arrange
        bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        var config = new Config()
        {
            WebDriverPath = isLinux? "resources/chromedriver" : "resources/chromedriver.exe",
            WebDriverPort = 9915
        };
        var webDriver = new ChromeWebDriver(config);
        
        //act and assert
        webDriver.StartServer();
        Assert.True(webDriver.ServerRunning());
        
        webDriver.CloseServer();
        Assert.False(webDriver.ServerRunning());
    }
    */
}