namespace PhoneBookApp.Model.Dto;

public record class InterpretCommandDto
{
    public required string CommandInNaturalLanguage { get; init; }
}
