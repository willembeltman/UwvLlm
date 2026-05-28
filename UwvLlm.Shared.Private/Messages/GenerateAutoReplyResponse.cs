namespace UwvLlm.Shared.Private.Messages;

public record GenerateAutoReplyResponse(
    Guid MailMessageId,
    string? CookieData,
    string? SessionData,
    string? StateData);