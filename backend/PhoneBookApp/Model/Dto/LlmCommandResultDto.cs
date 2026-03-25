using System.Text.Json.Serialization;
using PhoneBookApp.Model.Enums;

namespace PhoneBookApp.Model.Dto;

public record class LlmCommandResultDto
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public LlmAction Action { get; init; }

    // who are we looking for? (to update or delete)
    public string? TargetName { get; init; }
    public string? TargetPhoneNumber { get; init; }

    public string? Name { get; init; }
    public string? PhoneNumber { get; init; }
}
