var builder = DistributedApplication.CreateBuilder(args);

//var sql = builder.AddSqlServer("sql")
//    .AddDatabase("DefaultConnection");

var rabbit = builder.AddRabbitMQ("rabbit");

// Storage API
var storage = builder.AddProject<Projects.UwvLlm_Storage>("storage")
    .WithExternalHttpEndpoints();

// Fabric (console app)
var fabric = builder.AddProject<Projects.UwvLlm_Fabric>("fabric");

// Llm (console app)
var llmproxy = builder.AddProject<Projects.UwvLlm_LlmProxy>("llmproxy")
    //.WithReference(sql)
    .WithReference(rabbit)
    //.WaitFor(sql)
    .WaitFor(rabbit);

// Core API
var api = builder.AddProject<Projects.UwvLlm_Api>("api")
    //.WithReference(sql)
    .WithReference(rabbit)
    .WithReference(storage)
    .WithReference(fabric)
    .WithReference(llmproxy)
    //.WaitFor(sql)
    .WaitFor(rabbit)
    .WithExternalHttpEndpoints();

llmproxy.WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
        .WithEnvironment("DOTNET_ENVIRONMENT", "Development");


// (optioneel) environment variables
api.WithEnvironment("STORAGE__BASEURL", storage.GetEndpoint("https"));
api.WithEnvironment("FABRIC__HOST", "localhost");
api.WithEnvironment("FABRIC__PORT", "9494");

//// Console app that behaves like a client and runs the app integration flow.
//builder.AddProject<Projects.UwvLlm_App_Core_IntegrationTest>("app-integration-test")
//    .WithReference(api)
//    .WaitFor(api)
//    .WaitFor(llmproxy)
//    .WithEnvironment("FrontendConfig__ApiBackendUrl", api.GetEndpoint("https"));

builder.Build().Run();
