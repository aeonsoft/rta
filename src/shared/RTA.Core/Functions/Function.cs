namespace RTA.Core.Functions;

public class Function
{
    public required string Name { get; init; }

    public Dictionary<string, Argument>? Arguments { get; } = new Dictionary<string, Argument>();
       
}
