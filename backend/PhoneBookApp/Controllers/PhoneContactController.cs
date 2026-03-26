using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneBookApp.Data;
using PhoneBookApp.Mappers;
using PhoneBookApp.Model.Dto;
using PhoneBookApp.Services;
using PhoneBookApp.Model.Enums;
using PhoneBookApp.Model.Entities;
using PhoneBookApp.Model.Common;

namespace PhoneBookApp.Controllers;

[Route("api/contacts")]
[ApiController]
public class PhoneContactController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly INaturalLanguageInterpreter _naturalLanguageInterpreter;
    private readonly IActionExecutorService _actionExecutor;

    public PhoneContactController(AppDbContext dbContext, INaturalLanguageInterpreter naturalLanguageInterpreter, IActionExecutorService actionExecutor)
    {
        this._dbContext = dbContext;
        this._naturalLanguageInterpreter = naturalLanguageInterpreter;
        this._actionExecutor = actionExecutor;
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

        ServiceResult executionResult = await _actionExecutor.ExecuteActionAsync(result);

        if(!executionResult.IsSuccess)
        {
            if(executionResult.IsNotFound)
                return NotFound(executionResult.ErrorMessage);
            else
                return BadRequest(executionResult.ErrorMessage);
        }
        
        return Ok(executionResult.Data);
    }

}