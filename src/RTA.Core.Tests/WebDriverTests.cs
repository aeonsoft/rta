using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using RTA.Core.WebDriver;
using RTA.Core.WebDriver.Commands;
using RTA.Core.WebDriver.Commands.DeleteSession;
using RTA.Core.WebDriver.Commands.ElementClick;
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
    const string SauceDemoUrl = "https://www.saucedemo.com/";
    private readonly Settings _settings = new Settings { Port = 9515 };
    private readonly HttpClient _httpClient = new HttpClient();


    /// <summary>
    /// Closes a WebDriver Session
    /// </summary>
    /// <param name="sessionId"></param>
    private async Task CloseSession(string sessionId)
    {
        var url = $"http://localhost:{_settings.Port}/session/{sessionId}";
        await _httpClient.DeleteAsync(url);
    }


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
    public async Task NavigateTo_ValidUrl_OpenRequestedPage()
    {
        //arrange
        var expectedUrl = SauceDemoUrl;
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
        var expectedUrl = SauceDemoUrl;
        var session = await new NewSessionCommand(_settings, _httpClient).RunAsync();
        Assert.NotNull(session);
        Assert.NotNull(session.SessionId);

        //act
        await new NavigateToCommand(_settings, _httpClient, session.SessionId, expectedUrl).RunAsync();
        var response = await new GetCurrentUrlCommand(_settings, _httpClient, session.SessionId).RunAsync();
        await CloseSession(session.SessionId);


        //assert
        Assert.NotNull(response);
        Assert.Contains(expectedUrl, response);
    }

    [Fact]
    public async Task FindElement_OnValidElement_ShouldReturnInternalElementId()
    {
        var session = await new NewSessionCommand(_settings, _httpClient).RunAsync();
        Assert.NotNull(session);
        Assert.NotNull(session.SessionId);

        //act
        await new NavigateToCommand(_settings, _httpClient, session.SessionId, SauceDemoUrl).RunAsync();
        var internalId =
            await new FindElementCommand(_settings, _httpClient, session.SessionId, "#user-name").RunAsync();
        await CloseSession(session.SessionId);


        //assert
        Assert.NotNull(internalId);
    }

    [Fact]
    public async Task FindElement_OnInvalidElement_ShouldReturnNull()
    {
        var session = await new NewSessionCommand(_settings, _httpClient).RunAsync();
        Assert.NotNull(session);
        Assert.NotNull(session.SessionId);

        //act
        await new NavigateToCommand(_settings, _httpClient, session.SessionId, SauceDemoUrl).RunAsync();
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
        const string expectedText = "some random text";
        const string targetElement = "#user-name";

        var session = await new NewSessionCommand(_settings, _httpClient).RunAsync();
        Assert.NotNull(session);
        Assert.NotNull(session.SessionId);

        //act
        await new NavigateToCommand(_settings, _httpClient, session.SessionId, SauceDemoUrl).RunAsync();
        var elementId =
            await new FindElementCommand(_settings, _httpClient, session.SessionId, targetElement).RunAsync();

        Assert.NotNull(elementId);

        await new ElementSendKeysCommand(_settings, _httpClient, session.SessionId, elementId, expectedText).RunAsync();
        var foundText =
            await new GetElementAttributeCommand(_settings, _httpClient, session.SessionId, elementId, "value")
                .RunAsync();

        await CloseSession(session.SessionId);

        // assert
        Assert.NotNull(foundText);
        Assert.Equal(expectedText, foundText);
    }


    [Fact]
    public async Task ClickOnLogin_WithInvalidUserName_ShouldDisplayError()
    {
        //arrange
        const string userName = "non_existing_user_name";
        const string password = "secret_sauce";
        const string exceptedMessage = "Epic sadface: Username and password do not match any user in this service";
        const string errorMessageSelector = "h3[data-test='error']";
        const string userNameSelector = "#user-name";
        const string pwdSelector = "#password";
        const string loginButtonSelector = "#login-button";

        var session = await new NewSessionCommand(_settings, _httpClient).RunAsync();
        Assert.NotNull(session);
        Assert.NotNull(session.SessionId);

        var sessionId = session.SessionId;
        var element = new Dictionary<string, string?>();

        //act
        await new NavigateToCommand(_settings, _httpClient, sessionId, SauceDemoUrl).RunAsync();
        element[userNameSelector] =
            await new FindElementCommand(_settings, _httpClient, sessionId, userNameSelector).RunAsync();
        element[pwdSelector] = await new FindElementCommand(_settings, _httpClient, sessionId, pwdSelector).RunAsync();
        element[loginButtonSelector] =
            await new FindElementCommand(_settings, _httpClient, sessionId, loginButtonSelector).RunAsync();
        var errorElement =
            await new FindElementCommand(_settings, _httpClient, sessionId, errorMessageSelector).RunAsync();

        Assert.NotNull(element[userNameSelector]);
        Assert.NotNull(element[pwdSelector]);
        Assert.NotNull(element[loginButtonSelector]);
        Assert.Null(errorElement); // the element must not be present in the page at this moment

        await new ElementSendKeysCommand(_settings, _httpClient, sessionId, element[userNameSelector], userName)
            .RunAsync();
        await new ElementSendKeysCommand(_settings, _httpClient, sessionId, element[pwdSelector], password).RunAsync();
        await new ElementClickCommand(_settings, _httpClient, sessionId, element[loginButtonSelector]).RunAsync();

        // the error message should be visible on screen
        errorElement = element[errorMessageSelector] =
            await new FindElementCommand(_settings, _httpClient, sessionId, errorMessageSelector).RunAsync();
        Assert.NotNull(errorElement);

        var foundMessage = await new GetElementTextCommand(_settings, _httpClient, sessionId, errorElement).RunAsync();
        await CloseSession(session.SessionId);

        // assert
        Assert.Equal(exceptedMessage, foundMessage);
    }

    [Fact]
    public async Task ClickOnLogin_WithValidCredentials_ShouldLogAndRedirect()
    {
        //arrange
        const string userName = "standard_user";
        const string password = "secret_sauce";
        const string exceptedUrl = "https://www.saucedemo.com/inventory.html";

        var session = await new NewSessionCommand(_settings, _httpClient).RunAsync();
        Assert.NotNull(session);
        Assert.NotNull(session.SessionId);

        var sessionId = session.SessionId;
        var elements = new Dictionary<string, string?>();

        //act
        await new NavigateToCommand(_settings, _httpClient, sessionId, SauceDemoUrl).RunAsync();
        elements["#user-name"] =
            await new FindElementCommand(_settings, _httpClient, sessionId, "#user-name").RunAsync();
        elements["#password"] = await new FindElementCommand(_settings, _httpClient, sessionId, "#password").RunAsync();
        elements["#login-button"] =
            await new FindElementCommand(_settings, _httpClient, sessionId, "#login-button").RunAsync();

        Assert.NotNull(elements["#user-name"]);
        Assert.NotNull(elements["#password"]);
        Assert.NotNull(elements["#login-button"]);

        await new ElementSendKeysCommand(_settings, _httpClient, sessionId, elements["#user-name"], userName)
            .RunAsync();
        await new ElementSendKeysCommand(_settings, _httpClient, sessionId, elements["#password"], password).RunAsync();
        await new ElementClickCommand(_settings, _httpClient, sessionId, elements["#login-button"]).RunAsync();

        var currentUl = await new GetCurrentUrlCommand(_settings, _httpClient, sessionId).RunAsync();
        await CloseSession(session.SessionId);

        // assert
        Assert.NotNull(currentUl);
        Assert.Equal(exceptedUrl, currentUl);

    }

}