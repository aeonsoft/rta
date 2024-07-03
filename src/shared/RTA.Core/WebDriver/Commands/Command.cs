namespace RTA.Core.WebDriver.Commands;

public interface ICommand<T>
{
    public Task<T?> RunAsync();

}