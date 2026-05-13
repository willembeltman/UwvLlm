namespace UwvLlm.Infrastructure.Llm.Models;

public record ToolCallFunction(
    string Name,
    ToolCallFunctionArguments Arguments);