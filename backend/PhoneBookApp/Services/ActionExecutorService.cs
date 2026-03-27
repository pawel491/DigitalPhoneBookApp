using System;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using PhoneBookApp.Data;
using PhoneBookApp.Mappers;
using PhoneBookApp.Model.Common;
using PhoneBookApp.Model.Dto;
using PhoneBookApp.Model.Entities;
using PhoneBookApp.Model.Enums;

namespace PhoneBookApp.Services;

public class ActionExecutorService : IActionExecutorService
{
    private readonly AppDbContext _dbContext;
    public ActionExecutorService(AppDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<ServiceResult> ExecuteActionAsync(LlmCommandResultDto result)
    {
        string? validationError = ValidateCommandResult(result);
        if(validationError != null)
            return ServiceResult.BadRequest(validationError);

        switch(result.Action)
        {
            case LlmAction.Add:
                return await ExecuteAddActionAsync(result);
            case LlmAction.Delete:
                return await ExecuteDeleteActionAsync(result);
            case LlmAction.Get:
                return await ExecuteGetActionAsync(result);
            case LlmAction.Update:
                return await ExecuteUpdateActionAsync(result);
            default:
                return ServiceResult.BadRequest("AI assistant did not understand your command.");
        }
    }

    private async Task<ServiceResult> ExecuteAddActionAsync(LlmCommandResultDto result)
    {
        PhoneContact newContact = new PhoneContact
        {
            Name = result.Name!,
            PhoneNumber = result.PhoneNumber!
        };

        _dbContext.PhoneContacts.Add(newContact);
        await _dbContext.SaveChangesAsync();
        return ServiceResult.Success(new { Message = "Contact added successfully.", Contact = newContact.ToDto() });
    }

    private async Task<ServiceResult> ExecuteDeleteActionAsync(LlmCommandResultDto result)
    {
        var contactToDelete = await FindTargetContactAsync(result.TargetName, result.TargetPhoneNumber);
        if (contactToDelete == null)
            return ServiceResult.NotFound("No contact found matching the LLM's criteria for deletion.");

        _dbContext.PhoneContacts.Remove(contactToDelete);
        await _dbContext.SaveChangesAsync();
        return ServiceResult.Success(new { Message = "Contact deleted successfully.", Contact = (PhoneContactDto?)null });
    }

    private async Task<ServiceResult> ExecuteGetActionAsync(LlmCommandResultDto result)
    {
        var contact = await FindTargetContactAsync(result.TargetName, result.TargetPhoneNumber);
        if (contact == null)
            return ServiceResult.NotFound("No contact found matching the criteria.");

        return ServiceResult.Success(new {Message = "Found contact successfully.", Contact = contact.ToDto()});
    }

    private async Task<ServiceResult> ExecuteUpdateActionAsync(LlmCommandResultDto result)
    {
        var contactToUpdate = await FindTargetContactAsync(result.TargetName, result.TargetPhoneNumber);
        if (contactToUpdate == null)
            return ServiceResult.NotFound("No contact found matching the criteria for update.");

        if(!string.IsNullOrEmpty(result.Name))
            contactToUpdate.Name = result.Name;
        if(!string.IsNullOrEmpty(result.PhoneNumber))
            contactToUpdate.PhoneNumber = result.PhoneNumber;

        await _dbContext.SaveChangesAsync();
        return ServiceResult.Success(new { Message = "Contact updated successfully.", Contact = contactToUpdate.ToDto() });
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
                    return "AI assistant did not return necessary information to add a contact.";
                if(!IsValidPhoneNumber(result.PhoneNumber))
                    return "AI assistant returned an invalid phone number format.";
                break;
            case LlmAction.Update:
                if(!string.IsNullOrEmpty(result.PhoneNumber) && !IsValidPhoneNumber(result.PhoneNumber))
                    return "AI assistant returned an invalid phone number format for the update.";
                if(string.IsNullOrEmpty(result.TargetName) && string.IsNullOrEmpty(result.TargetPhoneNumber))
                    return $"AI assistant did not return necessary information to identify the target contact to {result.Action.ToString().ToLower()}.";
                break;
            case LlmAction.Delete:
            case LlmAction.Get:
                if(string.IsNullOrEmpty(result.TargetName) && string.IsNullOrEmpty(result.TargetPhoneNumber))
                    return $"AI assistant did not return necessary information to identify the target contact to {result.Action.ToString().ToLower()}.";
                break;
        } 
        return null;
    }
    private bool IsValidPhoneNumber(string phoneNumber)
    {
        return Regex.IsMatch(phoneNumber, @"^\+?[0-9\s\-()]{7,15}$");
    }
}
