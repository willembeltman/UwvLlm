using gAPI.Core.Client;
using gAPI.Core.Interfaces;
using gAPI.Generated;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UwvLlm.App.Core.Interfaces;
using UwvLlm.App.Core.Services;
using UwvLlm.Shared.Public;
using UwvLlm.Shared.Public.Dtos;
using UwvLlm.Shared.Public.Interfaces;

var builder = Host.CreateApplicationBuilder(args);

var apiBackendUrl = builder.Configuration["FrontendConfig:ApiBackendUrl"]
    ?? "https://localhost:7281";

builder.Services.AddAutoApiClient();
builder.Services.AddAutoSseClient();
builder.Services.AddAuthenticationServices<State>(apiBackendUrl);
builder.Services.AddScoped<IStateParser<State>, StateParser>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ConsoleAppServices>();
builder.Services.AddScoped<IUiService>(sp => sp.GetRequiredService<ConsoleAppServices>());
builder.Services.AddScoped<IDispatcherService>(sp => sp.GetRequiredService<ConsoleAppServices>());
builder.Services.AddScoped<INavigationService>(sp => sp.GetRequiredService<ConsoleAppServices>());
builder.Services.AddScoped<INavigationManager>(sp => sp.GetRequiredService<ConsoleAppServices>());
builder.Services.AddScoped<IntegrationNotificationHub>();
builder.Services.AddScoped<INotificationHub>(sp => sp.GetRequiredService<IntegrationNotificationHub>());
builder.Services.AddScoped<IntegrationScenario>();

using var host = builder.Build();
using var scope = host.Services.CreateScope();

await scope.ServiceProvider
    .GetRequiredService<IntegrationScenario>()
    .RunAsync();
