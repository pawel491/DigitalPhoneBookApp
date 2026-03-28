namespace PhoneBookApp.Services;

public interface IContactValidator
{
    Task<string?> CheckForDuplicatesAsync(string? name, string? phoneNumber, int? excludeId = null);
    bool IsValidPhoneNumber(string phoneNumber);
}