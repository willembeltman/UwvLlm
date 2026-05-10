using UwvLlm.Shared.Public.Dtos;

namespace UwvLlm.Infrastructure.Messaging.Messages;

public record GenerateAutoReplyRequest(
    MailMessage Email);
