using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using RTA.Core.WebDriver;
using RTA.Core.WebDriver.Commands;
using RTA.Core.WebDriver.Commands.DeleteSession;
using RTA.Core.WebDriver.Commands.ElementSendKeys;
using RTA.Core.WebDriver.Commands.FindElement;
using RTA.Core.WebDriver.Commands.GetCurrentUrl;
using RTA.Core.WebDriver.Commands.GetElementAttribute;
using RTA.Core.WebDriver.Commands.GetElementText;
using RTA.Core.WebDriver.Commands.NavigateTo;
using RTA.Core.WebDriver.Commands.NewSession;

namespace RTA.Core.Tests;

public class WebDriverTests
{
    private readonly Settings _settings = new Settings
    {
        Port = 9515
    };
    
    private readonly HttpClient _httpClient = new HttpClient();


    /// <summary>
    /// Closes a WebDriver Session
    /// </summary>
    /// <param name="sessionId"></param>
    private async Task CloseSession(string sessionId)
    {
        var url = $"http://localhost:{_settings.Port}/session/{sessionId}";
        await _httpClient.DeleteAsync(url); }
    
     
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
        Assert.Equal("chrome", result?.Capabilities?.BrowserName);

        //cleanup
        if (!string.IsNullOrWhiteSpace(result?.SessionId))
        {
            await CloseSession(result.SessionId);
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

    [Fact]
    public async Task NavigateToCommand_OpenRequestedPage()
    {
        //arrange
        var expectedUrl = "https://www.google.com/";
        var session = await new NewSessionCommand(_settings, _httpClient).RunAsync();
        Assert.NotNull(session);
        Assert.NotNull(session.SessionId);
        
        //act
        var navigate = await new NavigateToCommand(_settings, _httpClient, session.SessionId, expectedUrl).RunAsync();
        var getUrlUrl = $"http://localhost:{_settings.Port}/session/{session.SessionId}/url";  
        var response = await _httpClient.GetAsync(getUrlUrl);
        var content = await response.Content.ReadAsStringAsync();
        await CloseSession(session.SessionId);
        
        
        //assert
        Assert.True(navigate);
        Assert.Contains(expectedUrl, content);
    }

    [Fact]
    public async Task GetCurrentUrl_ReturnsCurrentUrl()
    {
        var expectedUrl = "https://www.google.com/";
        var session = await new NewSessionCommand(_settings, _httpClient).RunAsync();
        Assert.NotNull(session);
        Assert.NotNull(session.SessionId);
        
        //act
        await new NavigateToCommand(_settings, _httpClient, session.SessionId, expectedUrl).RunAsync();
        var response = await new GetCurrentUrlCommand(_settings, _httpClient, session.SessionId).RunAsync();
        await CloseSession(session.SessionId);
        
        
        //assert
        Assert.NotNull(response);
        Assert.Contains(expectedUrl, response.Value);
    }

    [Fact]
    public async Task FindElement_OnValidElement_ShouldReturnInternalElementId()
    {
        var expectedUrl = "https://www.saucedemo.com/";
        var session = await new NewSessionCommand(_settings, _httpClient).RunAsync();
        Assert.NotNull(session);
        Assert.NotNull(session.SessionId);
        
        //act
        await new NavigateToCommand(_settings, _httpClient, session.SessionId, expectedUrl).RunAsync();
        var internalId =
            await new FindElementCommand(_settings, _httpClient, session.SessionId, "#user-name").RunAsync();
        await CloseSession(session.SessionId);
        
        
        //assert
        Assert.NotNull(internalId);
    }
    
    [Fact]
    public async Task FindElement_OnInvalidElement_ShouldReturnNull()
    {
        var expectedUrl = "https://www.saucedemo.com/";
        var session = await new NewSessionCommand(_settings, _httpClient).RunAsync();
        Assert.NotNull(session);
        Assert.NotNull(session.SessionId);
        
        //act
        await new NavigateToCommand(_settings, _httpClient, session.SessionId, expectedUrl).RunAsync();
        var internalId =
            await new FindElementCommand(_settings, _httpClient, session.SessionId, "#non-existing-id").RunAsync();
        await CloseSession(session.SessionId);
        
        
        //assert
        Assert.Null(internalId);
    }


    [Fact]
    public async Task SendKeys_OnValidElement_ShouldFillElement()
    {
        //arrange
        const string expectedUrl = "https://www.saucedemo.com/";
        const string expectedText = "some random text";
        const string targetElement = "#user-name";
        
        var session = await new NewSessionCommand(_settings, _httpClient).RunAsync();
        Assert.NotNull(session);
        Assert.NotNull(session.SessionId);
        
        //act
        await new NavigateToCommand(_settings, _httpClient, session.SessionId, expectedUrl).RunAsync();
        var elementId =
            await new FindElementCommand(_settings, _httpClient, session.SessionId, targetElement).RunAsync();

        Assert.NotNull(elementId);

        await new ElementSendKeysCommand(_settings, _httpClient, session.SessionId, elementId, expectedText).RunAsync();
        var foundText = await new GetElementAttributeCommand(_settings, _httpClient, session.SessionId, elementId, "value")
            .RunAsync();
        
        await CloseSession(session.SessionId);
        
        // assert
        Assert.NotNull(foundText);
        Assert.Equal(expectedText, foundText);
    } 
    
    
}