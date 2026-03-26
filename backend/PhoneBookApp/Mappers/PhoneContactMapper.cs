using System;
using PhoneBookApp.Model.Dto;
using PhoneBookApp.Model.Entities;

namespace PhoneBookApp.Mappers;

public static class PhoneContactMapper
{
    public static PhoneContactDto ToDto(this PhoneContact entity)
    {
        return new PhoneContactDto(
            entity.Id,
            entity.Name,
            entity.PhoneNumber
        );
    }
    public static PhoneContact ToEntity(this CreatePhoneContactDto phoneContactDto)
    {
        return new PhoneContact
        {
            Name = phoneContactDto.Name,
            PhoneNumber = phoneContactDto.PhoneNumber
        };
    }
    public static void UpdateEntity(this PhoneContact entity, CreatePhoneContactDto phoneContactDto)
    {
        entity.Name = phoneContactDto.Name;
        entity.PhoneNumber = phoneContactDto.PhoneNumber;
    }
}
