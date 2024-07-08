using System.Text.Json.Serialization;

namespace RTA.Core.WebDriver.Commands.GetElementText;

public record GetElementTextResponse
{
    [JsonPropertyName("value")]
    public string? Value { get; init; }   
}