using UwvLlm.Shared.Public.Dtos;

namespace UwvLlm.Shared.Private.Messages;

public record GenerateAutoReplyRequest(
    Guid MailMessageId,
    string? CookieData,
    string? SessionData,
    string? StateData);
