using gAPI.Core.Client;
using gAPI.Core.Interfaces;
using UwvLlm.App.Core.Interfaces;
using UwvLlm.Shared.Public.CrudInterfaces;
using UwvLlm.Shared.Public.Dtos;

internal sealed class IntegrationScenario(
    IAuthenticationService authenticationService,
    IAuthenticatedHttpClient<State> authenticatedHttpClient,
    IUsersCrudService usersCrudService,
    IEmailService emailService,
    IClientConnection clientConnection,
    IntegrationNotificationHub notificationHub)
{
    public async Task RunAsync()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(1));

        var testId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var userName = $"integration-{testId}";
        var email = $"{userName}@example.local";
        const string password = "Test123!";

        Console.WriteLine("Running UwvLlm app integration test.");
        Console.WriteLine($"Registering {email}...");

        await RetryAsync(
            () => authenticationService.RegisterAsync(userName, email, password, password),
            timeout.Token);

        if (!await authenticatedHttpClient.IsAuthenticatedAsync(timeout.Token))
            throw new InvalidOperationException("Registration completed, but the client is not authenticated.");

        Console.WriteLine("Opening notification channel...");
        clientConnection.SubscribeAsync(notificationHub);

        var users = await usersCrudService.List(0, int.MaxValue, null, timeout.Token);
        if (!users.Success || users.Response == null || users.Response.Length == 0)
            throw new InvalidOperationException($"Could not load users after registration. Error: {users.Error}");

        var recipient = users.Response.FirstOrDefault(a => a.Email == email)
            ?? users.Response.First();

        Console.WriteLine($"Sending email to {recipient.Email}...");
        await emailService.Send(
            recipient.Id,
            "Integration test auto reply",
            "Hoi, dit is een integratietest. Wil je kort automatisch antwoorden?");

        Console.WriteLine("Waiting up to 1 minute for the LLM auto-reply notification...");

        var notification = await notificationHub.WaitForNotificationAsync(timeout.Token);

        if (!notification.Message.Contains("AUTO-REPLY", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("A notification arrived, but it did not contain the expected auto-reply marker.");

        Console.WriteLine("Integration test passed.");
        Console.WriteLine($"Notification: {notification.Title}");
    }

    private static async Task RetryAsync(Func<Task> action, CancellationToken ct)
    {
        Exception? lastException = null;

        while (!ct.IsCancellationRequested)
        {
            try
            {
                await action();
                return;
            }
            catch (Exception ex) when (!ct.IsCancellationRequested)
            {
                lastException = ex;
                await Task.Delay(TimeSpan.FromSeconds(2), ct);
            }
        }

        throw new TimeoutException("The API did not become available before the integration test timed out.", lastException);
    }
}
