using UwvLlm.Shared.Public.Dtos;
using UwvLlm.Shared.Public.Interfaces;

namespace UwvLlm.App.Core.IntegrationTest;

internal sealed class IntegrationNotificationHub : INotificationHub
{
    private readonly TaskCompletionSource<UserNotification> NotificationReceived =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public void OnNotificationReceived(UserNotification notification)
    {
        NotificationReceived.TrySetResult(notification);
    }

    public async Task<UserNotification> WaitForNotificationAsync(CancellationToken ct)
    {
        await using var registration = ct.Register(
            () => NotificationReceived.TrySetCanceled(ct));

        return await NotificationReceived.Task;
    }
}
