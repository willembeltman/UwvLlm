using gAPI.Generated;
using UwvLlm.Shared.Public.CrudInterfaces;
using UwvLlm.Shared.Public.Dtos;
using UwvLlm.Shared.Public.Enums;
using UwvLlm.Shared.Public.Interfaces;

namespace UwvLlm.Api.Core.Services;

public class MailApi(
    IAuthenticationService authenticationService,
    IMailMessagesCrudService mailService,
    IUserNotificationsCrudService notificationService,
    IBusContext busContext,
    IClientContext clientContext)
    : IMailApi
{
    public async Task SendMail(MailMessage newMail, CancellationToken ct)
    {
        if (authenticationService.State.User == null)
            throw new Exception("User not logged in");

        // Set current user
        newMail.FromUserId = authenticationService.State.User.Id;

        // Create new email in db
        var mailMessageResponse = await mailService.Create(newMail, ct);
        var mailMessage = mailMessageResponse.ThrowIfNull();

        // Get autoresponse
        mailMessage.AutoResponse = await busContext.AutoReplyGeneratorBus
            .ToAny(timeOut: TimeSpan.FromMinutes(5))
            .GetAutoReply(mailMessage, ct);

        // Save autoresponse
        mailMessageResponse = await mailService.Update(mailMessage, ct);
        mailMessage = mailMessageResponse.ThrowIfNull();

        // Create new notification dto for the user
        var userNotification = CreateNotification(mailMessage);

        // Create new notification in db
        var notificationResult = await notificationService.Create(userNotification, ct);
        var notification = notificationResult.ThrowIfNull();

        // Send notification to the user
        clientContext.NotificationHub.ToAll().OnNotificationReceived(notification);
    }
    private static UserNotification CreateNotification(MailMessage mailMessage)
        => new()
        {
            UserId = mailMessage.ToUserId,
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