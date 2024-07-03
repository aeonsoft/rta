namespace RTA.Core.Functions;

public abstract class Function
{
    public required string Name { get; init; }

    public Arguments? Arguments { get; init; } = new Arguments();

    public virtual (bool sucess, string[]? errors) Run(Dictionary<string, object> arguments)
    {
        throw new NotImplementedException();
    }
}