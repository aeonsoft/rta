using System.Dynamic;

namespace RTA.Core.WebDriver.Commands.NewSession;

public record NewSessionResponse
{ 
    public string? SessionId { get; init; }
    public SessionCapabilities? Capabilities { get; init; }
}

public record SessionCapabilities
{
    public bool AcceptInsecureCerts { get; init; }
    public string? BrowserName { get; init; }
    public string? BrowserVersion { get; init; }
    public bool? NetworkConnectionEnabled { get; init; }
    public string? PageLoadStrategy { get; init; }
    public string? PlatformName { get; init; }
    public SessionTimeouts? Timeouts { get; init; }
}

public record SessionTimeouts {
    public uint? Implicit { get; init; }
    public uint? PageLoad{ get; init; }
    public uint? Script{ get; init; }
}