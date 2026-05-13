using gAPI.Core.Server.Extensions;
using gAPI.Core.Extensions;
using gAPI.Core.ServiceBus.Interfaces;
using gAPI.Core.ServiceBus.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UwvLlm.Infrastructure.Llm.Clients;
using UwvLlm.Infrastructure.Llm.Interfaces;
using UwvLlm.LlmProxy.Core.Handlers;
using UwvLlm.LlmProxy.Extensions;
using gAPI.Core.ServiceBus.Extensions;

var builder = Host.CreateApplicationBuilder(args);
var serverConfig = builder.Configuration.CreateServerConfig();

builder.Services.AddStorage(serverConfig);
builder.Services.AddCommenServices(serverConfig);
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddSingleton<IConsoleService, ConsoleService>();
builder.Services.AddSingleton<ILlmClient, OllamaClient>();

builder.Services.AddSingleton<IRabbitConnectionProvider, RabbitConnectionProvider>();
builder.Services.AddSingleton<IHandlerRegistry, HandlerRegistry>();
builder.Services.AddSingleton<IServiceBusReceiver, ServiceBusReceiver>();
builder.Services.AddSingleton<IServiceBusSender, ServiceBusSender>();

builder.Services.AddTransient<GenerateAutoReplyRequestHandler>();

var app = builder.Build();
await app.StartConsoleWithServiceBusAsync();