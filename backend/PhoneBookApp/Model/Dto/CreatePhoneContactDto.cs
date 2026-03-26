namespace PhoneBookApp.Model.Dto;

public record class CreatePhoneContactDto(
    string Name,
    string PhoneNumber
);