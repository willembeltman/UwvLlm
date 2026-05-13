using UwvLlm.Infrastructure.Llm.Enums;

namespace UwvLlm.Infrastructure.Llm.Models;

public record Message(
    Role Role,
    string? ToolCallId,
    string? Content,
    string? Thinking,
    ToolCall[]? ToolCalls);
