using UwvLlm.Infrastructure.Llm.Models;

namespace UwvLlm.Infrastructure.Llm.Interfaces;

public interface ILlmClient : IDisposable
{
    bool Initialized { get; }

    Task<Model[]> GetModels(CancellationToken ct = default);
    Task InitializeModelAsync(Model model, CancellationToken ct = default);
    Task<LlmResponse> ChatAsync(Model model, LlmRequest apiCall, CancellationToken ct = default, bool? think = null);

    string CreateMessagesJson(Message[] messages);
    string CreateRequestJson(Model model, LlmRequest apiCall, bool? think = null);
    string CreateToolsJson(Tool[] tools);
}