namespace RTA.Core.Functions;

public abstract class Function
{
    public required string Name { get; init; }

    public Argument? Arguments { get; init; } = new Argument ();

    public virtual (bool sucess, string[]? errors) Run(Dictionary<string, object> arguments)
    {
        throw new NotImplementedException();
    }
}
