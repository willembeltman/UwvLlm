using UwvLlm.App.Core.Interfaces;
using UwvLlm.Shared.Public.Dtos;
using UwvLlm.Shared.Public.Interfaces;

namespace UwvLlm.App.Core.Services;

public class EmailService(
    IUiService uiService,
    IMailApi mailApi,
    INavigationService navigationService) 
    : IEmailService
{
    public async Task Send(Guid? toUserId, string? subject, string? body, CancellationToken ct)
    {
        if (toUserId == null)
        {
            await uiService.ShowAlertAsync("Fout", "to verplicht", "OK");
            return;
        }
        if (string.IsNullOrWhiteSpace(subject))
        {
            await uiService.ShowAlertAsync("Fout", "subject verplicht", "OK");
            return;
        }
        if (string.IsNullOrWhiteSpace(body))
        {
            await uiService.ShowAlertAsync("Fout", "body verplicht", "OK");
            return;
        }

        var mail = new MailMessage()
        {
            ToUserId = toUserId.Value,
            Subject = subject,
            Content = body
        };
        mailApi.SendMail(mail, ct);

        await uiService.ShowAlertAsync("OK", $"Mail verstuurd naar {toUserId}", "OK");
        await navigationService.GotoMainPageAsync();
    }
}
