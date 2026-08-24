using gAPI.Core.Interfaces;
using gAPI.Core.Server.Authentication;
using gAPI.Core.Server.Extensions;
using gAPI.Core.Server.Mappings;
using gAPI.Core.ServiceBus.Extensions;
using gAPI.Llm.Client.Clients;
using gAPI.Llm.Client.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UwvLlm.Infrastructure.Data.CrudServices;
using UwvLlm.Infrastructure.Data.Extensions;
using UwvLlm.Infrastructure.Data.Mappings;
using UwvLlm.Shared.Public;
using UwvLlm.Shared.Public.CrudInterfaces;
using UwvLlm.Shared.Public.Dtos;

var builder = Host.CreateApplicationBuilder(args);
var serverConfig = builder.Configuration.CreateServerConfig();

// DIT MOET IN DE ANALYZER
builder.Services.AddStorage(serverConfig);
builder.Services.AddCommenServices(serverConfig); // API Config injection + TimeProvider
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddAuthenticationServices<UwvLlm.Infrastructure.Data.Entities.User, State>();
builder.Services.AddScoped<IStateMapping<UwvLlm.Infrastructure.Data.Entities.User, State>, StateMapping>();
builder.Services.AddScoped<IStateUserMapping<UwvLlm.Infrastructure.Data.Entities.User, StateUser>, StateUserMapping>();
builder.Services.AddScoped<IStateParser<State>, StateParser>();
// DIT MOET IN DE ANALYZER

//builder.Services.AddAutoApiServices(); // niet toegankelijk, want staat in gAPI.AutoApi.Server, die hem normaal genereerd
builder.Services.AddScoped<IMailMessagesCrudService, MailMessagesCrudService>();

builder.Services.AddCrudMappings(); // Wordt fysiek gegenereerd
builder.Services.AddCrudUseCases(); // Wordt fysiek gegenereerd
builder.Services.AddSingleton<ILlmClient, OllamaClient>();

builder.Services.AddServiceBus();

var app = builder.Build();
await app.StartConsoleWithServiceBusAsync();
