namespace UwvLlm.Infrastructure.Llm.Models;

public record LlmRequest(
    Message[] Messages,
    Tool[] Tools);
