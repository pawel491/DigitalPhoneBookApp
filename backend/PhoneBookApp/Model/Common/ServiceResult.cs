using System;

namespace PhoneBookApp.Model.Common;

public class ServiceResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsNotFound { get; set; }
    public object? Data { get; set; }

    public static ServiceResult Success(object? data = null) => new ServiceResult { IsSuccess = true, Data = data };
    public static ServiceResult BadRequest(string msg) => new ServiceResult { IsSuccess = false, ErrorMessage = msg };
    public static ServiceResult NotFound(string msg) => new ServiceResult { IsSuccess = false, IsNotFound = true, ErrorMessage = msg };
}
