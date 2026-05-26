using gAPI.Core.Extensions;
using gAPI.Core.Interfaces;
using gAPI.Core.Server;
using gAPI.Core.Server.Extensions;
using gAPI.Core.Server.Mappings;
using gAPI.Core.ServiceBus.Extensions;
using gAPI.Core.ServiceBus.Interfaces;
using gAPI.Core.ServiceBus.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UwvLlm.Api.Extensions;
using UwvLlm.Infrastructure.Data.Mappings;
using UwvLlm.Infrastructure.Llm.Clients;
using UwvLlm.Infrastructure.Llm.Interfaces;
using UwvLlm.LlmProxy.Core.Handlers;
using UwvLlm.LlmProxy.Extensions;
using UwvLlm.Shared.Public;
using UwvLlm.Shared.Public.Dtos;

var builder = Host.CreateApplicationBuilder(args);
var serverConfig = builder.Configuration.CreateServerConfig();

builder.Services.AddStorage(serverConfig);
builder.Services.AddCommenServices(serverConfig);
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddAuthenticationServices<UwvLlm.Infrastructure.Data.Entities.User, State>();
builder.Services.AddScoped<IStateMapping<UwvLlm.Infrastructure.Data.Entities.User, State>, StateMapping>();
builder.Services.AddScoped<IStateUserMapping<UwvLlm.Infrastructure.Data.Entities.User, StateUser>, StateUserMapping>();
builder.Services.AddScoped<IStateParser<State>, StateParser>();
builder.Services.AddCrudMappings();
builder.Services.AddCrudUseCases();
builder.Services.AddSingleton<IConsoleService, ConsoleService>();
builder.Services.AddSingleton<ILlmClient, OllamaClient>();

builder.Services.AddSingleton<IRabbitConnectionProvider, RabbitConnectionProvider>();
builder.Services.AddSingleton<IHandlerRegistry, HandlerRegistry>();
builder.Services.AddSingleton<IServiceBusReceiver, ServiceBusReceiver>();
builder.Services.AddSingleton<IServiceBusSender, ServiceBusSender>();

builder.Services.AddTransient<GenerateAutoReplyRequestHandler>();

var app = builder.Build();
await app.StartConsoleWithServiceBusAsync();