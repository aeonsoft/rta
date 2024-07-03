using RTA.Core.Functions;

namespace RTA.Core.Tests;

public class Test
{
    public enum Section { Act, Arrange, Assert };

    public string Name { get; set; } = "New Test Case";
    public string? Version { get; set; }
    public string? Description { get; set; }

    public Dictionary<string, Arguments>? Arrange { get; set; }
    public Dictionary<string, Arguments> Act { get; set; } = new Dictionary<string, Arguments>();
    public Dictionary<string, Arguments> Assert { get; set; } = new Dictionary<string, Arguments>();

}

