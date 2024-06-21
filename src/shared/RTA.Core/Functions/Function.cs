namespace RTA.Core.Functions;

public class Function
{
    public required string Name { get; set; }

    public IList<Argument> Arguments { get; } = new List<Argument>();
}