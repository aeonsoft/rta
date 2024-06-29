using RTA.Core.Functions;

namespace RTA.Core.Tests;

public class Test
{
    public enum Section { Act, Arrange, Assert };

    public string Name { get; set; } = "New Test Case";
    public string? Version { get; set; }
    public string? Description { get; set; }

    public Dictionary<string, Argument>? Arrange { get; set; }
    public Dictionary<string, Argument> Act { get; set; } = new Dictionary<string, Argument>();
    public Dictionary<string, Argument> Assert { get; set; } = new Dictionary<string, Argument>();

}

