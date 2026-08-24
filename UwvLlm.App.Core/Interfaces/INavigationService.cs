using gAPI.Core.Client.Interfaces;

namespace UwvLlm.App.Core.Interfaces;

public interface INavigationService : IUriNavigationManager
{
    Task NavigateToAsync<TPage>();
    Task GotoSendEmailPage();
    Task OpenNotifications();
    Task GotoMainPageAsync();
    Task GotoRegisterPageAsync();
    Task GotoLoginPageAsync();
}