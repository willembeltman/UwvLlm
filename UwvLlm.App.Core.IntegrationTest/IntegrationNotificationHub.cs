using UwvLlm.Shared.Public.Dtos;
using UwvLlm.Shared.Public.Interfaces;

internal sealed class IntegrationNotificationHub : INotificationHub
{
    private readonly TaskCompletionSource<UserNotification> NotificationReceived =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task OnNotificationReceived(UserNotification notification)
    {
        NotificationReceived.TrySetResult(notification);
        return Task.CompletedTask;
    }

    public async Task<UserNotification> WaitForNotificationAsync(CancellationToken ct)
    {
        await using var registration = ct.Register(
            () => NotificationReceived.TrySetCanceled(ct));

        return await NotificationReceived.Task;
    }
}
