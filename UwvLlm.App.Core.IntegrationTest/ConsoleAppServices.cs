using UwvLlm.App.Core.Interfaces;

namespace UwvLlm.App.Core.IntegrationTest;

internal sealed class ConsoleAppServices : IUiService, IDispatcherService, INavigationService
{
    private string CurrentPath { get; set; } = "/";

    public Task ShowAlert(string title, string message, string cancel)
    {
        Console.WriteLine($"{title}: {message}");
        return Task.CompletedTask;
    }

    public void Invoke(Action action)
        => action();

    public string GetPathAndQuery()
        => CurrentPath.TrimStart('/');

    public Task NavigateToAsync<TPage>()
        => Task.CompletedTask;

    public Task GotoSendEmailPage()
    {
        CurrentPath = "/email";
        return Task.CompletedTask;
    }

    public Task OpenNotifications()
    {
        CurrentPath = "/notifications";
        return Task.CompletedTask;
    }

    public Task GotoMainPageAsync()
    {
        CurrentPath = "/";
        return Task.CompletedTask;
    }

    public Task GotoRegisterPageAsync()
    {
        CurrentPath = "/register";
        return Task.CompletedTask;
    }

    public Task GotoLoginPageAsync()
    {
        CurrentPath = "/login";
        return Task.CompletedTask;
    }
}
