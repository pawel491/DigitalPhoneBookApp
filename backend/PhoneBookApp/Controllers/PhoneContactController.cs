using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneBookApp.Data;
using PhoneBookApp.Mappers;
using PhoneBookApp.Model.Dto;
using PhoneBookApp.Services;
using PhoneBookApp.Model.Enums;
using PhoneBookApp.Model.Entities;

namespace PhoneBookApp.Controllers;

[Route("api/contacts")]
[ApiController]
public class PhoneContactController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly INaturalLanguageInterpreter _naturalLanguageInterpreter;

    public PhoneContactController(AppDbContext dbContext, INaturalLanguageInterpreter naturalLanguageInterpreter)
    {
        this._dbContext = dbContext;
        this._naturalLanguageInterpreter = naturalLanguageInterpreter;
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

    [HttpPost("ask")]
    public async Task<IActionResult> AskQuestion([FromBody] InterpretCommandDto commandDto)
    {
        LlmCommandResultDto result = await _naturalLanguageInterpreter.InterpretAsync(commandDto.CommandInNaturalLanguage);
        string? validationError = ValidateCommandResult(result);
        if(validationError != null)
            return BadRequest(validationError);

        switch (result.Action)
        {
            case LlmAction.Add:
                return await HandleAddAsync(result);
            case LlmAction.Delete:
                return await HandleDeleteAsync(result);
            case LlmAction.Get:
                return await HandleGetAsync(result);
            case LlmAction.Update:
                return await HandleUpdateAsync(result);
            default:
                return BadRequest("LLM did not understand your command.");
        }
    }


    private async Task<IActionResult> HandleAddAsync(LlmCommandResultDto result)
    {
        PhoneContact newContact = new PhoneContact
        {
            Name = result.Name!,
            PhoneNumber = result.PhoneNumber!
        };

        _dbContext.PhoneContacts.Add(newContact);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    private async Task<IActionResult> HandleDeleteAsync(LlmCommandResultDto result)
    {
        var contactToDelete = await FindTargetContactAsync(result.TargetName, result.TargetPhoneNumber);
        if (contactToDelete == null)
            return NotFound("No contact found matching the LLM's criteria for deletion.");

        _dbContext.PhoneContacts.Remove(contactToDelete);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    private async Task<IActionResult> HandleGetAsync(LlmCommandResultDto result)
    {
        var contact = await FindTargetContactAsync(result.TargetName, result.TargetPhoneNumber);
        if (contact == null)
            return NotFound("No contact found matching the LLM's criteria.");

        return Ok();
    }

    private async Task<IActionResult> HandleUpdateAsync(LlmCommandResultDto result)
    {
        var contactToUpdate = await FindTargetContactAsync(result.TargetName, result.TargetPhoneNumber);
        if (contactToUpdate == null)
            return NotFound("No contact found matching the LLM's criteria for update.");

        if(!string.IsNullOrEmpty(result.Name))
            contactToUpdate.Name = result.Name;
        if(!string.IsNullOrEmpty(result.PhoneNumber))
            contactToUpdate.PhoneNumber = result.PhoneNumber;

        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    private async Task<PhoneContact?> FindTargetContactAsync(string? targetName, string? targetPhoneNumber)
    {
        if(string.IsNullOrEmpty(targetName) && string.IsNullOrEmpty(targetPhoneNumber))
            return null;

        var contact = await _dbContext.PhoneContacts
            .Where(pc => (!string.IsNullOrEmpty(targetName) && pc.Name.ToLower() == targetName.ToLower()) ||
                         (!string.IsNullOrEmpty(targetPhoneNumber) && pc.PhoneNumber == targetPhoneNumber))
            .FirstOrDefaultAsync();

        return contact;
    }

    private string? ValidateCommandResult(LlmCommandResultDto result)
    {
        switch(result.Action)
        {
            case LlmAction.Add:
                if(string.IsNullOrEmpty(result.Name) || string.IsNullOrEmpty(result.PhoneNumber))
                    return "LLM did not return necessary information to add a contact.";
                break;
            case LlmAction.Delete:
            case LlmAction.Get:
            case LlmAction.Update:
                if(string.IsNullOrEmpty(result.TargetName) && string.IsNullOrEmpty(result.TargetPhoneNumber))
                    return $"LLM did not return necessary information to identify the target contact to {result.Action.ToString().ToLower()}.";
                break;
        } 
        return null;
    }
}