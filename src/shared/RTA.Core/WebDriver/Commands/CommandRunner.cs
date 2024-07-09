using System.Diagnostics.CodeAnalysis;
using RTA.Core.WebDriver.Commands.DeleteSession;
using RTA.Core.WebDriver.Commands.ElementClick;
using RTA.Core.WebDriver.Commands.ElementSendKeys;
using RTA.Core.WebDriver.Commands.FindElement;
using RTA.Core.WebDriver.Commands.GetCurrentUrl;
using RTA.Core.WebDriver.Commands.GetElementAttribute;
using RTA.Core.WebDriver.Commands.GetElementText;
using RTA.Core.WebDriver.Commands.NavigateTo;
using RTA.Core.WebDriver.Commands.NewSession;

namespace RTA.Core.WebDriver.Commands;


public class CommandRunnerException(string? message) : Exception(message);

/// <summary>
/// Runs commands against a web driver in its own session.
/// Use one instance per thread if you want multiple sessions in parallel
/// </summary>
public class CommandRunner(Settings settings, HttpClient httpClient)
{
    private readonly Settings _settings = settings;
    private readonly HttpClient _httpClient = httpClient;
    private string? _sessionId = null;

    private bool InSession => _sessionId is not null;
    private string SessionId => _sessionId ?? string.Empty;

    public async Task<string?> StartSession()
    {
        if (InSession)
            await CloseSession();
        
        var session = await new NewSessionCommand(_settings, _httpClient).RunAsync();
        _sessionId = session ?.SessionId;
        return _sessionId;
    }

    /// <summary>
    /// Check if we have a session.
    /// If not, throws an exception.
    /// </summary>
    /// <exception cref="CommandRunnerException"></exception>
    private void EnsureSession()
    {
        if (!InSession)
            throw new CommandRunnerException($"No active session detected");
    }

    public async Task<bool> CloseSession()
    {
        if (!InSession) 
            return true;
        
        var result = await new DeleteSessionCommand(_settings, _httpClient, SessionId)
            .RunAsync();
        
        if (result)
            _sessionId = null;
        return result;
    }

    public async Task<bool> NavigateTo(string where)
    {
        EnsureSession();
        return await new NavigateToCommand(_settings, _httpClient, SessionId, where)
            .RunAsync();
    }

    /// <summary>
    /// Finds one element using a css selector.
    /// </summary>
    /// <param name="selector"></param>
    /// <returns>Internal element's Id</returns>
    public async Task<string?> FindElement(string selector)
    {
        EnsureSession();
        return await new FindElementCommand(_settings, _httpClient, SessionId, selector)
            .RunAsync();
    }

    /// <summary>
    /// Send string as keystrokes to a html element
    /// </summary>
    /// <param name="element">element reference returned by FindElement</param>
    /// <param name="keys">value to send as keystrokes</param>
    public async Task<bool> SendKeys(string element, string keys)
    {
        EnsureSession();
        return await new ElementSendKeysCommand(_settings, _httpClient, SessionId, element, keys)
            .RunAsync();
    }

    /// <summary>
    /// Sends a mouse click to a html element
    /// </summary>
    /// <param name="element">element reference returned by FindElement</param>
    public async Task<bool> Click(string element)
    {
        EnsureSession();
        return await new ElementClickCommand(_settings, _httpClient, SessionId, element)
            .RunAsync();
    }

    /// <summary>
    /// Retrieve the current URL displayed on session's browser
    /// </summary>
    public async Task<string?> GetCurrentUrl()
    {
        EnsureSession();
        return await new GetCurrentUrlCommand(_settings, _httpClient, SessionId)
            .RunAsync();
    }

    /// <summary>
    /// Retrieve the text value of a html element 
    /// </summary>
    /// <param name="element">element reference returned by FindElements</param>
    public async Task<string?> GetText(string element)
    {
        EnsureSession();
        return await new GetElementTextCommand(_settings, _httpClient, SessionId, element)
            .RunAsync();
    }
    
    /// <summary>
    /// Retrieve an attribute of a html element
    /// </summary>
    /// <param name="element">element reference returned by FindElements</param>
    /// <param name="attribute">element's attribute name</param>
    /// <returns></returns>
    public async Task<string?> GetAttribute(string element, string attribute)
    {
        EnsureSession();
        return await new GetElementAttributeCommand(_settings, _httpClient, SessionId, element, attribute)
            .RunAsync();
    }
    
    
    
}