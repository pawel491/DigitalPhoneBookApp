using System;
using PhoneBookApp.Model.Dto;

namespace PhoneBookApp.Services;

public interface INaturalLanguageInterpreter
{
    Task<LlmCommandResultDto> InterpretAsync(string commandInNaturalLanguage);
}
