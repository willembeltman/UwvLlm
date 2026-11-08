using gAPI.Core.Attributes;
using UwvLlm.Shared.Public.Dtos;

namespace UwvLlm.LlmProxy.Core.Handlers;

[GenerateBus]
public interface IGenerateAutoReplyRequestHandler
{
    Task<MailMessage> Handle(MailMessage message, CancellationToken ct);
}