using Groq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

public class GroqService
{
    private readonly GroqClient _client;
    private readonly string? _primaryModel;
    private readonly string[] _fallbackModels;

    public GroqService(IConfiguration config)
    {
        var apiKey = config["Groq:ApiKey"];
        _client = new GroqClient(apiKey);
        _primaryModel = config["Groq:Model"];
        _fallbackModels = config.GetSection("Groq:FallbackModels").Get<string[]>() ?? System.Array.Empty<string>();
    }

    public async Task<string> GeneratePlan(string prompt)
    {
        var candidates = new List<string>();
        if (!string.IsNullOrWhiteSpace(_primaryModel)) candidates.Add(_primaryModel);
        candidates.AddRange(_fallbackModels);

        foreach (var model in candidates)
        {
            if (string.IsNullOrWhiteSpace(model)) continue;

            try
            {
                var response = await _client.Chat.CreateChatCompletionAsync(
                    new List<Groq.ChatCompletionRequestMessage>
                    {
                        new Groq.ChatCompletionRequestMessage
                        {
                            User = new Groq.ChatCompletionRequestUserMessage
                            {
                                Content = prompt
                            }
                        }
                    },
                    model: model
                );

                return response.Choices[0].Message.Content;
            }
            catch (InvalidOperationException ex) when (ex.Message?.Contains("model_not_found") == true
                                                       || ex.Message?.Contains("does not exist") == true
                                                       || ex.Message?.Contains("decommissioned") == true)
            {
                // try next candidate
                continue;
            }
            catch (HttpRequestException)
            {
                throw; // network problems - bubble up
            }
        }

        throw new InvalidOperationException(
            $"Configured Groq model(s) not available: {string.Join(", ", candidates)}. " +
            "Check the Groq console for available models and verify the API key has access.");
    }
}