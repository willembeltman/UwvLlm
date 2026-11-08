using gAPI.Generated;
using gAPI.Storage.Server.Extensions;
using System.Globalization;

var invariantCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentCulture = invariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = invariantCulture;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
}
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddStorageServer(builder.Configuration);
builder.Services.AddAutoBus(builder.Configuration);

var app = builder.Build();
app.MapStorageServer();
app.Run();