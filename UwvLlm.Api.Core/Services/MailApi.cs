using gAPI.Core.Server;
using gAPI.Core.ServiceBus.Interfaces;
using UwvLlm.Infrastructure.Messaging.Messages;
using UwvLlm.Shared.Public.CrudInterfaces;
using UwvLlm.Shared.Public.Dtos;
using UwvLlm.Shared.Public.Interfaces;

namespace UwvLlm.Api.Core.Services;

public class MailApi(
    IAuthenticationService<Infrastructure.Data.Entities.User, State> authenticationService,
    IMailMessageCrudService mailService,
    IServiceBusSender serviceBusSender)
    : IMailApi
{
    public async Task SendMail(MailMessage newMail, CancellationToken ct)
    {
        if (authenticationService.State.User == null)
            return;

        newMail.FromUserId = authenticationService.State.User.Id;
        var mailResponse = await mailService.Create(newMail, ct);
        if (mailResponse.Success == false || mailResponse.Response == null) 
            return;

        var autoReplyMessage = new GenerateAutoReplyRequest(
            mailResponse.Response);

        await serviceBusSender.SendAsync("LlmProxy", autoReplyMessage, ct);
    }
}
