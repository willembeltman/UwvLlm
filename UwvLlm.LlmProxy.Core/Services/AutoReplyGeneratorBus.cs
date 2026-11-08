using gAPI.Generated;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using UwvLlm.Shared.Private.Interfaces;
using UwvLlm.Shared.Public.Dtos;

namespace UwvLlm.LlmProxy.Core.Services;

public class AutoReplyGeneratorBus(
    IBusConnection busConnection,
    IChatClient chatClient)
    : IAutoReplyGeneratorBus
    , IHostedService
{
    private CancellationTokenSource? Cts;
    async Task IHostedService.StartAsync(CancellationToken ct)
    {
        if (Cts != null)
            return;
        Cts = new CancellationTokenSource();
        ct = Cts.Token;
        await busConnection.SubscribeAsync(this, ct);
    }
    async Task IHostedService.StopAsync(CancellationToken ct)
    {
        if (Cts == null)
            return;
        await busConnection.UnsubscribeAsync(this, ct);
        Cts.Cancel();
        Cts.Dispose();
        Cts = null;
    }
    public async Task<string> GetAutoReply(MailMessage mailMessage, CancellationToken ct)
    {
        var systemPrompt = """
        Create a reply to this email conversation. Use the same language as the user uses.
        You MUST respond strictly with a valid JSON object matching this schema:
        {
            "Content": "text of the reply"
        }
        Do not include markdown formatting like ```json or any other text outside the JSON object.
        """;

        var mailMessageText = $@"Date: {mailMessage.Date}
From: {mailMessage.FromUserName}
To: {mailMessage.ToUserName}
Subject: {mailMessage.Subject}

{mailMessage.Content}";

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, mailMessageText)
        };

        var options = new ChatOptions
        {
            ModelId = "gpt-oss:20b",
            ResponseFormat = ChatResponseFormat.Json
        };

        // Gecorrigeerde aanroep: GetResponseAsync in plaats van CompleteAsync
        ChatResponse response = await chatClient.GetResponseAsync(messages, options, ct);

        // Schoon de response op mocht het model stiekem toch markdown backticks meesturen
        var cleanText = response.Messages.Last().Text?.Trim();
        if (cleanText != null && cleanText.StartsWith("```"))
        {
            cleanText = cleanText.Replace("```json", "").Replace("```", "").Trim();
        }

        // Parse de JSON-output om de 'Content' waarde op te halen
        if (!string.IsNullOrEmpty(cleanText))
        {
            using var doc = JsonDocument.Parse(cleanText);
            if (doc.RootElement.TryGetProperty("Content", out var contentProperty))
            {
                return contentProperty.GetString() ?? string.Empty;
            }
        }

        throw new Exception($"Failed to generate auto reply. Raw response was: {cleanText}");
    }
}
