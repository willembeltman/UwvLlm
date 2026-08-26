using gAPI.Core.Client.Interfaces;
using gAPI.Generated;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UwvLlm.App.Core.IntegrationTest;
using UwvLlm.App.Core.Interfaces;
using UwvLlm.App.Core.Services;
using UwvLlm.Shared.Public.Interfaces;

var builder = Host.CreateApplicationBuilder(args);

var apiBackendUrl = builder.Configuration["FrontendConfig:ApiBackendUrl"]
    ?? "https://localhost:7281";

builder.Services.AddAutoApiSseClient(apiBackendUrl);

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ConsoleAppServices>();
builder.Services.AddScoped<IUiService>(sp => sp.GetRequiredService<ConsoleAppServices>());
builder.Services.AddScoped<IDispatcherService>(sp => sp.GetRequiredService<ConsoleAppServices>());
builder.Services.AddScoped<INavigationService>(sp => sp.GetRequiredService<ConsoleAppServices>());
builder.Services.AddScoped<IUriNavigationManager>(sp => sp.GetRequiredService<ConsoleAppServices>());
builder.Services.AddScoped<IntegrationNotificationHub>();
builder.Services.AddScoped<INotificationHub>(sp => sp.GetRequiredService<IntegrationNotificationHub>());
builder.Services.AddScoped<IntegrationScenario>();

using var host = builder.Build();
using var scope = host.Services.CreateScope();

await scope.ServiceProvider
    .GetRequiredService<IntegrationScenario>()
    .RunAsync();
