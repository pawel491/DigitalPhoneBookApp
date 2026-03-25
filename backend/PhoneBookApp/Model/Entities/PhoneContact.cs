using System;

namespace PhoneBookApp.Model.Entities;

public class PhoneContact
{
    public int Id {get; set;}
    public required string Name {get; set;}
    public required string PhoneNumber {get; set;}
}
