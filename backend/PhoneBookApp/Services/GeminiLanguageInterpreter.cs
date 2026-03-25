using System;
using System.Text;
using System.Text.Json;
using PhoneBookApp.Model.Dto;
using PhoneBookApp.Model.Enums;

namespace PhoneBookApp.Services;

public class GeminiLanguageInterpreter : INaturalLanguageInterpreter
{
    private readonly HttpClient _httpClient;
    private readonly string _requestUrl;
    private readonly string _apiKey;

    private const string API_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent";
    private const string SystemPrompt = @"
            You are an assistant for a phone book application. Your SOLE task is to process user intent into a JSON format.
            Return ONLY a pure JSON object, without any Markdown formatting or extraneous text.
            Rules:
            - Action can only take the values: ""Add"", ""Update"", ""Delete"", ""Get"", ""Unknown"".
            - TargetName is the name of the contact we are looking for (in update, delete).
            - TargetPhoneNumber is the phone number we are looking for (in update, delete).
            - Name and PhoneNumber are the new data (in add or update).
            Example 1: ""Add to my phone book John. His phone number is 123456789"" -> {""Action"": ""Add"", ""Name"": ""John"", ""PhoneNumber"": ""123456789""}
            Example 2: ""Please add a record for Joanna with the number 222333444"" -> {""Action"": ""Add"", ""TargetName"": ""Joanna"", ""PhoneNumber"": ""222333444""}
            Example 3: ""What is the phone number for Joanna?"" -> {""Action"": ""Get"", ""TargetName"": ""Joanna""}
            Example 4: ""Delete the contact for Tom"" -> {""Action"": ""Delete"", ""TargetName"": ""Tom""}
            Example 5: ""Change Mark's phone number to +48 999888777"" -> {""Action"": ""Update"", ""TargetName"": ""Mark"", ""PhoneNumber"": ""+48 999888777""}
        ";

    public GeminiLanguageInterpreter(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["GeminiApiKey"] ?? throw new ArgumentNullException("GeminiApiKey is not configured");
        _requestUrl = $"{API_URL}?key={_apiKey}";
    }

    public async Task<LlmCommandResultDto> InterpretAsync(string commandInNaturalLanguage)
    {
        if (string.IsNullOrWhiteSpace(commandInNaturalLanguage))
            return new LlmCommandResultDto { Action = LlmAction.Unknown };
        
        // construct the request body according to Gemini API specifications
        var requestBody = new
        {
            system_instruction = new { parts = new[] { new { text = SystemPrompt } } },
            contents = new[]
            {
                new { parts = new[] { new { text = commandInNaturalLanguage } } }
            },
            generationConfig = new { response_mime_type = "application/json" } 
        };
        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(_requestUrl, content);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();

            // unpack the Gemini response to get the generated JSON string
            using var jsonDoc = JsonDocument.Parse(responseJson);

            if (!jsonDoc.RootElement.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
                return new LlmCommandResultDto { Action = LlmAction.Unknown };

            var extractedJsonString = jsonDoc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<LlmCommandResultDto>(extractedJsonString!, options);

            return result ?? new LlmCommandResultDto { Action = LlmAction.Unknown };
        }
        catch (Exception ex)
        {
            return new LlmCommandResultDto { Action = LlmAction.Unknown };
        }
    }
}
