using System;
using PhoneBookApp.Model.Common;
using PhoneBookApp.Model.Dto;

namespace PhoneBookApp.Services;

public interface IActionExecutorService
{
    Task<ServiceResult> ExecuteActionAsync(LlmCommandResultDto result);
}
