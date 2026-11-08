using gAPI.Generated;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OllamaSharp;
using System.Globalization;
using UwvLlm.LlmProxy.Core.Services;

var invariantCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentCulture = invariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = invariantCulture;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
}
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddAutoBus(builder.Configuration);
builder.Services.AddChatClient(new OllamaApiClient(
    new Uri("http://localhost:11434"),
    "gpt-oss:20b"
));
builder.Services.AddHostedService<AutoReplyGeneratorBus>();

var app = builder.Build();
await app.RunAsync();
