using gAPI.Generated;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using UwvLlm.Infrastructure.Data.Entities;
using UwvLlm.Infrastructure.Data.Extensions;
using gAPI.Storage.Client.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoApiServer(builder.Configuration);
builder.Services.AddAutoAuthServer(builder.Configuration);
builder.Services.AddAutoBus(builder.Configuration);
builder.Services.AddStorage(builder.Configuration);

builder.Services.AddOpenApi();
builder.Services.AddCrudMappings();
builder.Services.AddCrudUseCases();

var app = builder.Build();

app.MapAutoApiServer();
app.MapAutoAuthServer();
app.MapOpenApi();
app.MapScalarApiReference();
app.UseHttpsRedirection();

var val = builder.Configuration["CreateDatabase"];
if ((val ?? "").ToString().Equals("true", StringComparison.CurrentCultureIgnoreCase))
{
    using (var scope = app.Services.CreateScope())
    {
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        var db = factory.CreateDbContext();

        db.Database.Migrate();
    }
}

await app.RunAsync();