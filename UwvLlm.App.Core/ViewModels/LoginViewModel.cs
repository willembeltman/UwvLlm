using CommunityToolkit.Mvvm.Input;
using UwvLlm.App.Core.Interfaces;

namespace UwvLlm.App.Core.ViewModels;

public partial class LoginViewModel(
    IUiService iUiService,
    INavigationService navigationService,
    IAuthenticationService authenticationService)
    : BaseViewModel
{
    public string? Email { get => field; set => SetProperty(ref field, value); }
    public string? Password { get => field; set => SetProperty(ref field, value); }

    public async Task OnAppearingAsync()
    {
        var isAuthenticated = await authenticationService.IsAuthenticatedAsync();
        if (isAuthenticated == null)
            await iUiService.ShowAlertAsync("Cannot contact server", "Cannot contact server", "Cancel");

        if (isAuthenticated == true)
            await navigationService.GotoMainPageAsync();
    }

    [RelayCommand]
    public async Task Login()
        => await authenticationService.LoginAsync(Email, Password);

    [RelayCommand]
    public async Task GotoRegister()
        => await navigationService.GotoRegisterPageAsync();
}