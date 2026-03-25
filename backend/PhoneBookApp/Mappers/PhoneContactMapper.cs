using System;
using PhoneBookApp.Model.Dto;
using PhoneBookApp.Model.Entities;

namespace PhoneBookApp.Mappers;

public static class PhoneContactMapper
{
    public static PhoneContactDto ToDto(this PhoneContact phoneContact)
    {
        return new PhoneContactDto(
            phoneContact.Name,
            phoneContact.PhoneNumber
        );
    }
}
