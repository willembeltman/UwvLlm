using gAPI.Core.Server;
using gAPI.Core.ServiceBus.Interfaces;
using UwvLlm.Shared.Private.Messages;
using UwvLlm.Shared.Public.CrudInterfaces;
using UwvLlm.Shared.Public.Dtos;
using UwvLlm.Shared.Public.Interfaces;

namespace UwvLlm.Api.Core.Services;

public class MailApi(
    IAuthenticationService<Infrastructure.Data.Entities.User, State> authenticationService,
    IMailMessagesCrudService mailService,
    IServiceBusSender serviceBusSender)
    : IMailApi
{
    public async Task SendMail(MailMessage newMail, CancellationToken ct)
    {
        if (authenticationService.State.User == null)
            throw new Exception("User not logged in");

        newMail.FromUserId = authenticationService.State.User.Id;

        var mailMessageResponse = await mailService.Create(newMail, ct);
        var mailMessage = mailMessageResponse.ThrowIfNull();

        var autoReplyMessage = new GenerateAutoReplyRequest(
            mailMessage.Id,
            authenticationService.CookieData,
            authenticationService.SessionData,
            await authenticationService.GetStateDataAsync(ct));

        await serviceBusSender.SendAsync("LlmProxy", autoReplyMessage, ct);
    }
}