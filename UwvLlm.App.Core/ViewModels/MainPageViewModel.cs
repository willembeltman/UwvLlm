using CommunityToolkit.Mvvm.Input;
using gAPI.Generated;
using UwvLlm.App.Core.Interfaces;
using UwvLlm.Shared.Public.CrudInterfaces;

namespace UwvLlm.App.Core.ViewModels;

public partial class MainPageViewModel(
    IDispatcherService dispatcher,
    IClientConnection clientConnection,
    IUserNotificationsCrudService userNotificationsService,
    INavigationService navigationService,
    IUiService uiService) 
    : NotificationHubViewModel(dispatcher, clientConnection, userNotificationsService, navigationService, uiService)
{
    [RelayCommand]
    public async Task SendEmail()
        => await NavigationService.GotoSendEmailPage();
}
