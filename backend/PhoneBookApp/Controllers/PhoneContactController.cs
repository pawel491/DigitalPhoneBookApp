using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneBookApp.Data;
using PhoneBookApp.Mappers;
using PhoneBookApp.Model.Dto;

namespace PhoneBookApp.Controllers;

[Route("api/contacts")]
[ApiController]
public class PhoneContactController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    public PhoneContactController(AppDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetPhoneContacts()
    {
        var phoneContacts = await _dbContext.PhoneContacts.ToListAsync();

        var dtos = phoneContacts.Select(pc => pc.ToDto());
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPhoneContact(int id)
    {
        var phoneContact = await _dbContext.PhoneContacts.FindAsync(id);
        if (phoneContact == null)
        {
            return NotFound();
        }
        return Ok(phoneContact.ToDto());
    }
    
    [HttpPost]
    public async Task<IActionResult> CreatePhoneContact([FromBody] PhoneContactDto phoneContactDto)
    {
        var phoneContact = phoneContactDto.ToEntity();

        _dbContext.PhoneContacts.Add(phoneContact);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPhoneContact), new { id = phoneContact.Id }, phoneContact.ToDto());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePhoneContact([FromBody] PhoneContactDto phoneContactDto, int id)
    {
        var phoneContact = await _dbContext.PhoneContacts.FindAsync(id);
        if (phoneContact == null)
        {
            return NotFound();
        }

        phoneContact.UpdateEntity(phoneContactDto);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePhoneContact(int id)
    {
        var phoneContact = await _dbContext.PhoneContacts.FindAsync(id);
        if (phoneContact == null)
        {
            return NotFound();
        }

        _dbContext.PhoneContacts.Remove(phoneContact);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}
