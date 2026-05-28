namespace UwvLlm.Infrastructure.Llm.Models;

public record Tool(
    string Name,
    string Desciption,
    ToolParameter[] Parameters);
