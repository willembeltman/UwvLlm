using gAPI.Core.Attributes;
using UwvLlm.Shared.Public.Dtos;

namespace UwvLlm.Shared.Private.Interfaces;

[GenerateBus]
public interface IAutoReplyGeneratorBus
{
    Task<string> GetAutoReply(MailMessage mailMessage, CancellationToken ct);
}