namespace RTA.Core.WebDriver.Commands;

public record GetStatusResponse
{
    public bool Ready { get; init; }
    public string Message { get; init; }
}