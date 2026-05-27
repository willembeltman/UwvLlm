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
        // Check login
        var result = await authenticationService.InitializeAsync(
            "/Handlers/GenerateAutoReplyResponseHandler",
            message.CookieData,
            message.SessionData,
            message.StateData,
            ct);

        if (result.Forbidden || result.Authenticated == false)
            throw new Exception("Cannot find session");

        // Get the email with the auto-reply response
        var mailMessageResponse = await mailMessagesCrudService.Read(message.MailMessageId, ct);
        var mailMessage = mailMessageResponse.ThrowIfNull();

        // Create a new notification record for the user
        var userNotification = CreateNotification(mailMessage);

        // Create the notification 
        var notificationResult = await notificationService.Create(userNotification, ct);
        var notification = notificationResult.ThrowIfNull();

        // And send it to the user
        await notificationHub.ToAll.OnNotificationReceived(notification);
    }

    private static UserNotification CreateNotification(MailMessage mailMessage) 
        => new()
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
}
