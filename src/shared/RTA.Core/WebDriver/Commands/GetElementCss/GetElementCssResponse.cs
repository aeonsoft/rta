using System.Text.Json.Serialization;

namespace RTA.Core.WebDriver.Commands.GetElementCss;

public record GetElementCssResponse
{
    [JsonPropertyName("value")]
    public string? Value { get; init; }   
}