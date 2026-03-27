using System.ComponentModel.DataAnnotations;

namespace PhoneBookApp.Model.Dto;

public record class CreatePhoneContactDto(
    [Required(ErrorMessage = "Name is required.")]
    string Name,
    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^\+?[0-9\s\-()]{7,15}$", ErrorMessage = "Invalid phone number format.")]
    string PhoneNumber
);