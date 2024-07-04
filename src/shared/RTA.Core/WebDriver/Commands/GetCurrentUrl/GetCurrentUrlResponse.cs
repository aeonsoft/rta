using System.Text.Json.Serialization;

namespace RTA.Core.WebDriver.Commands.GetCurrentUrl;

public record GetCurrentUrlResponse
{
    [JsonPropertyName("value")]
    public string? Value { get; init; }
}