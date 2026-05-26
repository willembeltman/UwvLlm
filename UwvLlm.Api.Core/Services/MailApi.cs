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
            return;

        newMail.FromUserId = authenticationService.State.User.Id;
        var mailResponse = await mailService.Create(newMail, ct);
        if (mailResponse.Success == false || mailResponse.Response == null)
            throw new Exception(Enum.GetName(
                mailResponse.Error.HasValue 
                ? mailResponse.Error.Value 
                : gAPI.Core.Enums.BaseResponseErrorEnum.ErrorItemNotFound));
        var mailMessage = mailResponse.Response;

        var autoReplyMessage = new GenerateAutoReplyRequest(
            mailMessage.Id,
            authenticationService.CookieData,
            authenticationService.SessionData,
            await authenticationService.GetStateDataAsync(ct));

        await serviceBusSender.SendAsync("LlmProxy", autoReplyMessage, ct);
    }
}
