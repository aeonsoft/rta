namespace RTA.Core.Test;

public class TestCase
{
    public string Name { get; set; } = "New Test Case";
    public string? Version { get; set; }

    public Section Arrange { get; } = new Section();
    public Section Act { get; } = new Section();
    public Section Assert { get; }  = new Section();

}

