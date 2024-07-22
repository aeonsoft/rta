using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace RTA.Core.Tests;


/// <summary>
/// Logger for testing on xUnit framework.
/// This logger renders log messages to xUnit output window
/// </summary>
/// <typeparam name="T"></typeparam>
public class XLogger<T>(ITestOutputHelper output) : ILogger<T>, IDisposable
{
    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        output.WriteLine(message: state?.ToString());
    }

    public void Dispose()
    {
        
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return this;
    }

}
