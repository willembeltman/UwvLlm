namespace UwvLlm.Infrastructure.Llm.Models;

public record Model(
    string Name,
    long? MemorySize = null,
    int? MaxTokenSize = null,
    DateTime? LastModified = null);