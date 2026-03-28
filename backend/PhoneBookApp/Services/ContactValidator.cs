using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using PhoneBookApp.Data;

namespace PhoneBookApp.Services;

public class ContactValidator : IContactValidator
{
    private readonly AppDbContext _dbContext;
    public ContactValidator(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string?> CheckForDuplicatesAsync(string? name, string? phoneNumber, int? excludeId = null)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            bool nameExists = await _dbContext.PhoneContacts.AnyAsync(c => c.Name.ToLower() == name.ToLower() && c.Id != excludeId);
            if (nameExists) 
                return $"Contact with the name '{name}' already exists.";
        }

        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            bool phoneExists = await _dbContext.PhoneContacts.AnyAsync(c => c.PhoneNumber == phoneNumber && c.Id != excludeId);
            if (phoneExists) 
                return $"Phone number '{phoneNumber}' is already used.";
        }
        return null;
    }
    
    public bool IsValidPhoneNumber(string phoneNumber)
    {
        return Regex.IsMatch(phoneNumber, @"^\+?[0-9\s\-()]{7,15}$");
    }
}