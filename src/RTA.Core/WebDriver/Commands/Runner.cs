using RTA.Core.WebDriver.Commands;


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
    /// This method uses no caching strategy
    /// </summary>
    /// <param name="selector"></param>
    /// <returns>Internal element's Id</returns>
    private async Task<string?> FindElement(string selector)
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


    /// <summary>
    /// Try to find an element on page until reach the timeout.
    /// Use this method if you are dealing with ajax-based elements or values and need to make sure it
    /// exists on the page before interacting with it
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
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
            catch (Exception) {
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
        if (!_elements.TryGetValue(selector, out var elementRef))
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

    /// <summary>
    /// Takes a screenshot of current browsing context
    /// </summary>
    /// <returns></returns>
    public async Task<string?> ScreenShot()
    {
        EnsureSession();
        var image = await new ScreenShotCommand(settings, _httpClient, SessionId).RunAsync();
        return image;
    }
    
    /// <summary>
    /// Takes a screenshot of the elements
    /// </summary>
    /// <returns>Base64-encoded PNG image</returns>
    public async Task<string?> ElementScreenShot(string elementId)
    {
        EnsureSession();
        var elementRef = await FindElement(elementId);
        var image = await new ElementScreenShot(settings, _httpClient, SessionId, elementRef).RunAsync();
        return image;
    }

    /// <summary>
    /// Retrieves the id of the active element on the page.
    /// If there is no active element return null.
    /// If the element has an id, return it, otherwise return null
    /// </summary>
    /// <returns></returns>
    public async Task<string?> ActiveElementId()
    {
        EnsureSession();
        var elementRef = await new GetActiveElementCommand(settings, _httpClient, SessionId).RunAsync();
        if (elementRef is null) 
            return null;

        var elementId = await new GetElementAttributeCommand(settings, _httpClient, SessionId, elementRef, "id")
            .RunAsync();

        return elementId;
    }
    
    /// <summary>
    /// Retrieves the tag name of the active element on the page.
    /// If there is no active element return null.
    /// </summary>
    /// <returns></returns>
    public async Task<string?> ActiveElementTagName()
    {
        EnsureSession();
        var elementRef = await new GetActiveElementCommand(settings, _httpClient, SessionId).RunAsync();
        if (elementRef is null) 
            return null;

        var tagName = await new GetElementTagNameCommand(settings, _httpClient, SessionId, elementRef)
            .RunAsync();

        return tagName;
    }

    /// <summary>
    /// Retrieves the html source for current opened page
    /// </summary>
    /// <returns></returns>
    public async Task<string?> PageSource()
    {
        EnsureSession();
        return await new GetPageSourceCommand(settings, _httpClient, SessionId).RunAsync();
    }
    

    public void Dispose()
    {
        // ensure we do not keep an open socket
        _httpClient.Dispose();
    }
}