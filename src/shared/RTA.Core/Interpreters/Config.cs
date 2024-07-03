using System;

namespace RTA.Core.Interpreters;

public record Config
{
    public required string WebDriverPath { get; init; }
    public required uint WebDriverPort { get; init; }
}