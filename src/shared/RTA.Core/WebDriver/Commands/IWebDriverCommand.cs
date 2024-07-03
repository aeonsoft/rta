namespace RTA.Core.WebDriver.Commands;

public interface IWebDriverCommand
{
    Task<Response<T>> RunAsync<T>();
}