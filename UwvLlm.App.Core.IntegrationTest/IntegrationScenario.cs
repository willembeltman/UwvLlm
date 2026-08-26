using gAPI.Core.Client.Interfaces;
using gAPI.Generated;
using UwvLlm.App.Core.Interfaces;
using UwvLlm.Shared.Public.CrudInterfaces;

namespace UwvLlm.App.Core.IntegrationTest;

internal sealed class IntegrationScenario(
    IAuthenticationService authenticationService,
    IAuthenticatedHttpClient authenticatedHttpClient,
    IUsersCrudService usersCrudService,
    IEmailService emailService,
    ISseClientConnection clientConnection,
    IntegrationNotificationHub notificationHub)
{
    private static readonly TimeSpan SetupTimeout = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan LlmResponseTimeout = TimeSpan.FromMinutes(2);

    public async Task RunAsync()
    {
        using var setupTimeout = new CancellationTokenSource(SetupTimeout);

        var testId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var userName = $"integration-{testId}";
        var email = $"{userName}@example.local";
        const string password = "Test123!";

        Console.WriteLine("Running UwvLlm app integration test.");
        Console.WriteLine($"Registering {email}...");

        await RetryAsync(
            () => authenticationService.RegisterAsync(userName, email, password, password),
            setupTimeout.Token);

        if (await authenticatedHttpClient.IsAuthenticatedAsync(setupTimeout.Token) != true)
            throw new InvalidOperationException("Registration completed, but the client is not authenticated.");

        Console.WriteLine("Opening notification channel...");
        clientConnection.SubscribeAsync(notificationHub);

        var users = await usersCrudService.List(0, int.MaxValue, null, setupTimeout.Token);
        if (!users.Success || users.Response == null || users.Response.Length == 0)
            throw new InvalidOperationException($"Could not load users after registration. Error: {users.Error}");

        var recipient = users.Response.FirstOrDefault(a => a.Email == email)
            ?? users.Response.First();

        Console.WriteLine($"Sending email to {recipient.Email}...");
        await emailService.Send(
            recipient.Id,
            "Integration test auto reply",
            "Hoi, dit is een integratietest. Wil je kort automatisch antwoorden?");

        Console.WriteLine($"Waiting up to {LlmResponseTimeout.TotalMinutes:0} minute for the LLM auto-reply notification...");

        using var llmTimeout = new CancellationTokenSource(LlmResponseTimeout);
        var notification = await notificationHub.WaitForNotificationAsync(llmTimeout.Token);

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
