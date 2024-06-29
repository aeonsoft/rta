using RTA.Core.Interpreters;

namespace RTA.Core.Tests;

public class WebDriverTests
{
    [Fact]
    public void StartWebDriverServer_ShouldSpawnNewProcess()
    {
        //arrange
        var config = new Config()
        {
            WebDriverPath = "resources/chromedriver.exe",
            WebDriverPort = 9915
        };
        var webDriver = new ChromeWebDriver(config);
        
        //act and assert
        webDriver.StartServer();
        Assert.True(webDriver.ServerRunning());
        
        webDriver.CloseServer();
        Assert.False(webDriver.ServerRunning());
    }
}