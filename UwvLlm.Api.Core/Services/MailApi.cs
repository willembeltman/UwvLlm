using gAPI.Generated;
using UwvLlm.Shared.Public.CrudInterfaces;
using UwvLlm.Shared.Public.Dtos;
using UwvLlm.Shared.Public.Enums;
using UwvLlm.Shared.Public.Interfaces;

namespace UwvLlm.Api.Core.Services;

public class MailApi(
    IAuthenticationService authenticationService,
    IMailMessagesCrudService mailService,
    IGenerateAutoReplyRequestHandlerContext generateAutoReply,
    IUserNotificationsCrudService notificationService,
    INotificationHubContext notificationHub)
    : IMailApi
{
    public async Task SendMail(MailMessage newMail, CancellationToken ct)
    {
        if (authenticationService.State.User == null)
            throw new Exception("User not logged in");

        newMail.FromUserId = authenticationService.State.User.Id;

        var mailMessageResponse = await mailService.Create(newMail, ct);
        var mailMessage = mailMessageResponse.ThrowIfNull();

        mailMessage = await generateAutoReply.ToAny().Handle(mailMessage, ct);

        // Create a new notification record for the user
        var userNotification = CreateNotification(mailMessage);

        // Create the notification 
        var notificationResult = await notificationService.Create(userNotification, ct);
        var notification = notificationResult.ThrowIfNull();

        // And send it to the user
        notificationHub.ToAll().OnNotificationReceived(notification);
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