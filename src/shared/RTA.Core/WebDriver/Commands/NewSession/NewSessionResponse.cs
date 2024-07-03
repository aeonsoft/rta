using System.Dynamic;

namespace RTA.Core.WebDriver.Commands.NewSession;

public record NewSessionResponse
{
    public string? SessionId { get; init; }
    public ExpandoObject? Capabilities { get; init; }
}