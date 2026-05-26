using gAPI.Core.Enums;
using gAPI.Core.Server;
using gAPI.Core.ServiceBus.Interfaces;
using UwvLlm.Shared.Private.Messages;
using UwvLlm.Shared.Public.CrudInterfaces;
using UwvLlm.Shared.Public.Dtos;
using UwvLlm.Shared.Public.Enums;
using UwvLlm.Shared.Public.Interfaces;

namespace UwvLlm.Api.Core.Handlers;

public class GenerateAutoReplyResponseHandler(
    IAuthenticationService<Infrastructure.Data.Entities.User, State> authenticationService,
    IMailMessagesCrudService mailMessagesCrudService,
    IUserNotificationsCrudService notificationService,
    INotificationHubContext notificationHub) 
    : IHandler<GenerateAutoReplyResponse>
{
    public async Task Handle(GenerateAutoReplyResponse message, CancellationToken ct)
    {
        var result = await authenticationService.InitializeAsync(
            "/Handlers/GenerateAutoReplyResponseHandler",
            message.CookieData,
            message.SessionData,
            message.StateData,
            ct);

        if (result.Forbidden || result.Authenticated == false)
            throw new Exception("Cannot find session");

        var mailMessageResponse = await mailMessagesCrudService.Read(message.MailMessageId, ct);
        if (mailMessageResponse.Response == null || mailMessageResponse.Error.HasValue)
            throw new Exception(Enum.GetName(
                mailMessageResponse.Error.HasValue
                ? mailMessageResponse.Error.Value
                : BaseResponseErrorEnum.ErrorGettingData));
        var mailMessage = mailMessageResponse.Response;

        var userNotification = new UserNotification()
        {
            ExternalType = NotificationType.Mail,
            ExternalId = mailMessage.Id.ToString(), 
            Title = "Message received",
            Message = $@"Subject: {mailMessage.Subject}

{mailMessage.Content}

==========================
        AUTO-REPLY
==========================

{mailMessage.AutoResponse}

Do you want to auto-reply?",
            QuickOptions = ["Yes", "No", "Modify"]
        };

        var createResult = await notificationService.Create(userNotification, ct);
        if (createResult.Success == false || createResult.Response == null)
            throw new Exception("Could not make notification");

        await notificationHub.ToAll.OnNotificationReceived(createResult.Response);
    }
}
