namespace RTA.Core.Functions;

public record Argument
{
    public required string Name { get; init; }

    public DataType? DataType { get; init; }
    
    public object? Value { get; init; }
}