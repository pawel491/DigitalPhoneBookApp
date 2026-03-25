using System;
using PhoneBookApp.Model.Dto;
using PhoneBookApp.Model.Entities;

namespace PhoneBookApp.Mappers;

public static class PhoneContactMapper
{
    public static PhoneContactDto ToDto(this PhoneContact entity)
    {
        return new PhoneContactDto(
            entity.Name,
            entity.PhoneNumber
        );
    }
    public static PhoneContact ToEntity(this PhoneContactDto phoneContactDto)
    {
        return new PhoneContact
        {
            Name = phoneContactDto.Name,
            PhoneNumber = phoneContactDto.PhoneNumber
        };
    }
    public static void UpdateEntity(this PhoneContact entity, PhoneContactDto phoneContactDto)
    {
        entity.Name = phoneContactDto.Name;
        entity.PhoneNumber = phoneContactDto.PhoneNumber;
    }
}
