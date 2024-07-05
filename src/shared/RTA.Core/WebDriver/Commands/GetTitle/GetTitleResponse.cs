using System.Text.Json.Serialization;

namespace RTA.Core.WebDriver.Commands.GetTitle;

public record GetTitleResponse {
 
    [JsonPropertyName("value")]
    public string? Value { get; init; }   
}