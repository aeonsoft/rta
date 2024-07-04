using System.Net;
using System.Runtime.InteropServices;
using RTA.Core.Interpreters;
using RTA.Core.WebDriver;
using RTA.Core.WebDriver.Commands;
using RTA.Core.WebDriver.Commands.CloseSession;
using RTA.Core.WebDriver.Commands.NewSession;

namespace RTA.Core.Tests;

public class WebDriverTests
{
    private readonly Settings _settings = new Settings
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
        var command = new GetStatusCommand(_settings, _httpClient);

        var result = await command.RunAsync();

        Assert.NotNull(result);
        Assert.True(result.Ready);
    }
   
    
    [Fact]
    public async Task NewSessionCommand_ShouldRetrieveASessionId()
    {
        //arrange
        var command = new NewSessionCommand(_settings, _httpClient);
        
        //act
        var result = await command.RunAsync();

        //assert
        Assert.NotNull(result);
        Assert.NotNull(result.SessionId);
        Assert.Equal("chrome", result.Capabilities.BrowserName);

        //cleanup
        if (!string.IsNullOrWhiteSpace(result.SessionId))
        {
            // close the opened session       
            var url = $"http://localhost:9515/session/{result.SessionId}";
            await _httpClient.DeleteAsync(url);
        }
    }

    [Fact]
    public async Task CloseSession_ReturnsOk()
    {
        //arrange
        var newSession = new NewSessionCommand(_settings, _httpClient);
        
        //act
        var session = await newSession.RunAsync();
        Assert.NotNull(session);
        Assert.NotNull(session.SessionId);

        var closeSession = new DeleteSessionCommand(_settings, _httpClient, session.SessionId);
        var result = await closeSession.RunAsync();
        
        //assert
        Assert.True(result);
    }
}