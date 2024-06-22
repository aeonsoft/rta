using RTA.Core.Functions;
using YamlDotNet.Serialization;

namespace RTA.Core.Test;

public class Test
{
    public string Name { get; set; } = "New Test Case";
    public string? Version { get; set; }
    public string? Description { get; set; }

    public Dictionary<string, Argument>? Arrange { get; set; }
    public Dictionary<string, Argument> Act { get; set; } = new Dictionary<string, Argument>();
    public Dictionary<string, Argument> Assert { get; set; } = new Dictionary<string, Argument>();

}

