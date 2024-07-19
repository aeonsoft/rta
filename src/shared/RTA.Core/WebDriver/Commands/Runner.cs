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


public class RunnerException(string? message) : Exception(message);

/// <summary>
/// Runs commands against a web driver in its own session.
/// Use one instance per thread if you want multiple sessions in parallel
/// </summary>
public class Runner(Settings settings) : IDisposable
{
    private readonly HttpClient _httpClient = new HttpClient();
    private string? _sessionId = null;
    private readonly Dictionary<string, string?> _elements = new Dictionary<string, string?>();

    public bool InSession => _sessionId is not null;
    public string SessionId => _sessionId ?? string.Empty;
    

    public async Task<string?> StartSession()
    {
        if (InSession)
            await CloseSession();
        
        var session = await new NewSessionCommand(settings, _httpClient).RunAsync();
        _sessionId = session ?.SessionId;
        return _sessionId;
    }

    /// <summary>
    /// Check if we have a session.
    /// If not, throws an exception.
    /// </summary>
    /// <exception cref="RunnerException"></exception>
    private void EnsureSession()
    {
        if (!InSession)
            throw new RunnerException($"No active session detected");
    }

    public async Task<bool> CloseSession()
    {
        if (!InSession) 
            return true;
        
        var result = await new DeleteSessionCommand(settings, _httpClient, SessionId)
            .RunAsync();
        
        if (result)
            _sessionId = null;
        return result;
    }

    public async Task<bool> NavigateTo(string where)
    {
        EnsureSession();
        return await new NavigateToCommand(settings, _httpClient, SessionId, where)
            .RunAsync();
    }

    /// <summary>
    /// Finds one element using a css selector.
    /// This method uses no caching strategy.
    /// </summary>
    /// <param name="selector"></param>
    /// <returns>Internal element's Id</returns>
    public async Task<string?> FindElement(string selector)
    {
        EnsureSession();
        if (_elements.TryGetValue(selector, out var element))
            return element;
                
        element =  await new FindElementCommand(settings, _httpClient, SessionId, selector)
            .RunAsync();
        if (!String.IsNullOrEmpty(element))
            _elements[selector] = element;
        
        return element;
    }


    public async Task<bool> WaitForElement(string selector, uint timeout = 5000)
    {        
        var startTime = DateTime.Now;
        var endTime = startTime.AddMilliseconds(timeout);
        while(DateTime.Compare(startTime, endTime) < 0)
        {
            try
            {
                var elementRef = await FindElement(selector);
                if (elementRef is not null)
                {
                    return true;
                }
            }
            catch (Exception ex) {
                Thread.Sleep(300);
            }
        }

        return false;
    }



    /// <summary>
    /// Send string as keystrokes to a html element
    /// </summary>
    /// <param name="selector">css selector for element</param>
    /// <param name="keys">value to send as keystrokes</param>
    public async Task<bool> SendKeys(string selector, string keys)
    {
        EnsureSession();        
        var elementRef = await GetElementReference(selector) ?? string.Empty;
        return await new ElementSendKeysCommand(settings, _httpClient, SessionId, elementRef, keys)
            .RunAsync();

    }

    /// <summary>
    /// Finds one element using a css selector.
    /// This method uses the runner's internal cache for performance
    /// </summary>
    /// <param name="selector">css selector for element</param>
    /// <returns></returns>
    private async Task<string?> GetElementReference(string selector)
    {
        string? elementRef = null;
        if (!_elements.TryGetValue(selector, out elementRef))
        {
            elementRef = await FindElement(selector);
            if (elementRef is null)
                return null;
        }

        if (elementRef is not null)
            _elements[selector] = elementRef;

        return elementRef;
    }

    /// <summary>
    /// Sends a mouse click to a html element
    /// <param name="selector">css selector for element</param>
    /// </summary>
    public async Task<bool> Click(string selector)
    {
        EnsureSession();
        var elementRef = await GetElementReference(selector) ?? string.Empty;
        return await new ElementClickCommand(settings, _httpClient, SessionId, elementRef)
            .RunAsync();
    }

    /// <summary>
    /// Retrieve the current URL displayed on session's browser
    /// </summary>
    public async Task<string?> GetCurrentUrl()
    {
        EnsureSession();
        return await new GetCurrentUrlCommand(settings, _httpClient, SessionId)
            .RunAsync();
    }

    /// <summary>
    /// Retrieve the text value of a html element 
    /// </summary>
    /// <param name="selector">css selector for element</param>
    public async Task<string?> GetText(string selector)
    {
        EnsureSession();
        var elementRef = await GetElementReference(selector) ?? string.Empty;
        return await new GetElementTextCommand(settings, _httpClient, SessionId, elementRef)
            .RunAsync();
    }
    
    /// <summary>
    /// Retrieve an attribute of a html element
    /// </summary>
    /// <param name="selector">css selector for element</param>
    /// <param name="attribute">element's attribute name</param>
    /// <returns></returns>
    public async Task<string?> GetAttribute(string selector, string attribute)
    {
        EnsureSession();
        var elementRef = await GetElementReference(selector) ?? string.Empty;
        return await new GetElementAttributeCommand(settings, _httpClient, SessionId, elementRef, attribute)
            .RunAsync();
    }

    public void Dispose()
    {
        // ensure we do not keep an open socket
        _httpClient.Dispose();
    }
}