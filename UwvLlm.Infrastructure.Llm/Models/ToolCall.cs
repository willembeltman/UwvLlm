namespace UwvLlm.Infrastructure.Llm.Models;

public record ToolCall(
    string Id,
    ToolCallFunction Function);
