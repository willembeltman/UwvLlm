using gAPI.Core.Dtos;
using gAPI.Core.Enums;
using gAPI.Core.Server;
using gAPI.Core.ServiceBus.Interfaces;
using Microsoft.EntityFrameworkCore;
using UwvLlm.Infrastructure.Llm.Enums;
using UwvLlm.Infrastructure.Llm.Interfaces;
using UwvLlm.Infrastructure.Llm.Models;
using UwvLlm.Shared.Private.Messages;
using UwvLlm.Shared.Public.CrudInterfaces;
using UwvLlm.Shared.Public.Dtos;

namespace UwvLlm.LlmProxy.Core.Handlers;

public class GenerateAutoReplyRequestHandler(
    IMailMessagesCrudService mailMessagesCrudService,
    IAuthenticationService<Infrastructure.Data.Entities.User, State> authenticationService,
    IServiceBusSender sender,
    ILlmClient llmClient)
    : IHandler<GenerateAutoReplyRequest>
{
    public async Task Handle(GenerateAutoReplyRequest message, CancellationToken ct)
    {
        // Check login
        var result = await authenticationService.InitializeAsync(
            "/Handlers/GenerateAutoReplyResponseHandler",
            message.CookieData,
            message.SessionData,
            message.StateData,
            ct);

        if (result.Forbidden || result.Authenticated == false)
            throw new Exception("Cannot find session");

        // Get the email
        var mailMessageResponse = await mailMessagesCrudService.Read(message.MailMessageId, ct);
        var mailMessage = mailMessageResponse.ThrowIfNull();

        // Generate response
        mailMessage.AutoResponse = await GetAutoReply(mailMessage, ct);

        // Update the email
        mailMessageResponse = await mailMessagesCrudService.Update(mailMessage, ct);
        mailMessageResponse.ThrowIfNull();

        // Signal API to inform user
        var generateAutoReplyResponse = new GenerateAutoReplyResponse(
            message.MailMessageId,
            message.CookieData,
            message.SessionData,
            message.StateData);
        await sender.SendAsync("Api", generateAutoReplyResponse, ct);
    }


    private async Task<string> GetAutoReply(MailMessage mailMessage, CancellationToken ct)
    {
        // Hardcoded for now
        var model = new Model("gpt-oss:20b");
        if (llmClient.Initialized == false)
        {
            await llmClient.InitializeModelAsync(model, ct);
        }

        var systemPrompt = "Create a reply to this email conversation, use the same language as the user uses.";
        var mailMessageText = $@"Date: {mailMessage.Date}
From: {mailMessage.FromUserName}
To: {mailMessage.ToUserName}
Subject: {mailMessage.Subject}

{mailMessage.Content}";
        var messages = new List<Message>()
        {
            new(Role.System, null, systemPrompt, null, null),
            new(Role.User, null, mailMessageText, null, null)
        };

        var toolName = "reply-email";
        var tool = new Tool(toolName, "reply to the email", [new ToolParameter("Content", "string", "text of the reply")]);

        var autoResponse = (string?)null;
        while (autoResponse == null)
        {
            var request = new LlmRequest([.. messages], [tool]);
            var response = await llmClient.ChatAsync(model, request, ct);
            messages.Add(response.Message);

            autoResponse = response.Message.ToolCalls?
                .FirstOrDefault(a => a.Function.Name == toolName)?
                .Function.Arguments.Content;
        }
        return autoResponse;
    }
}
