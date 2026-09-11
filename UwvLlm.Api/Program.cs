using gAPI.Core.Server.Extensions;
using gAPI.Core.ServiceBus.Extensions;
using gAPI.Generated;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using UwvLlm.Infrastructure.Data.Entities;
using UwvLlm.Infrastructure.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);
var serverConfig = builder.Configuration.CreateServerConfig();

builder.Services.AddAutoApiServer(serverConfig);
builder.Services.AddAutoAuthServer(serverConfig);
builder.Services.AddStorage(serverConfig);
builder.Services.AddOpenApi();

// Extra services
builder.Services.AddCrudMappings();
builder.Services.AddCrudUseCases();

// Service Bus
builder.Services.AddServiceBus();

var app = builder.Build();

app.MapAutoApiServer();
app.MapAutoAuthServer();
app.UseHttpsRedirection();
app.MapOpenApi();
app.MapScalarApiReference();

// DIT MOET IN DE ANALYZER
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    var db = factory.CreateDbContext();

    db.Database.Migrate();
}
// DIT MOET IN DE ANALYZER

app.RunWithServiceBus(busName: "Api");