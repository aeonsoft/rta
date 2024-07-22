namespace RTA.Core.WebDriver.Commands;

public record Response<T>
{
    public T? Value { get; init; }
}