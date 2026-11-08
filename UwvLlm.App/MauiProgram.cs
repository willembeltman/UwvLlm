using gAPI.Core.Client.Interfaces;
using gAPI.Generated;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UwvLlm.App.Core.Interfaces;
using UwvLlm.App.Core.Services;
using UwvLlm.App.Core.ViewModels;
using UwvLlm.App.Pages;
using UwvLlm.App.Services;
using UwvLlm.Shared.Public.Interfaces;

namespace UwvLlm.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        using var stream = FileSystem
            .OpenAppPackageFileAsync("appsettings.Development.json")
            .GetAwaiter()
            .GetResult();
        if (stream != null)
            builder.Configuration.AddJsonStream(stream);
#else
        using var stream = FileSystem
            .OpenAppPackageFileAsync("appsettings.json")
            .GetAwaiter()
            .GetResult();
        if (stream != null)
            builder.Configuration.AddJsonStream(stream);
#endif

        builder.Services.AddAutoApiClient(builder.Configuration);
        builder.Services.AddAutoAuthClient(builder.Configuration);

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<EmailPage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<NotificationsPage>();
        builder.Services.AddTransient<RegisterPage>();

        builder.Services.AddScoped<NotificationHubViewModel>();
        builder.Services.AddScoped<EmailViewModel>();
        builder.Services.AddScoped<LoginViewModel>();
        builder.Services.AddScoped<MainPageViewModel>();
        builder.Services.AddScoped<RegisterViewModel>();
        builder.Services.AddScoped<NotificationPageViewModel>();

        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IDispatcherService, DispatcherService>();

        builder.Services.AddScoped<NavigationService>();
        builder.Services.AddScoped<INavigationService>(sp => sp.GetRequiredService<NavigationService>());
        builder.Services.AddScoped<IUriNavigationManager>(sp => sp.GetRequiredService<NavigationService>());
        builder.Services.AddScoped<INotificationHub>(sp => sp.GetRequiredService<NotificationPageViewModel>());
        builder.Services.AddScoped<IUiService, UiService>();

        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(EmailPage), typeof(EmailPage));
        Routing.RegisterRoute(nameof(NotificationsPage), typeof(NotificationsPage));

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
