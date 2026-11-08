var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .AddDatabase("DefaultConnection");

var rabbit = builder.AddRabbitMQ("ServiceBus")
    .WithManagementPlugin();

var fabric = builder.AddProject<Projects.UwvLlm_Fabric>("fabric")
    .WithEnvironment("Port", "9494")
    .WithEnvironment("BusPort", "9595");

// Storage API
var storage = builder.AddProject<Projects.UwvLlm_Storage>("storage")
    .WithReference(rabbit)
    .WaitFor(rabbit)
    .WithEnvironment("QueueName", "Standard")
    .WithEnvironment("BaseUrl", "https://localhost:7099")
    .WithExternalHttpEndpoints();

// Llm (console app)
var llmproxy = builder.AddProject<Projects.UwvLlm_LlmProxy>("llmproxy")
    .WithReference(rabbit)
    .WaitFor(rabbit)
    .WithEnvironment("QueueName", "Standard");

// Core API
var api = builder.AddProject<Projects.UwvLlm_Api>("api")
    .WithReference(sql)
    .WithReference(rabbit)
    .WithReference(storage)
    .WithReference(fabric)
    .WithReference(llmproxy)
    .WaitFor(sql)
    .WaitFor(rabbit)
    .WithEnvironment("CreateDatabase", "true")
    .WithEnvironment("UseMemoryDatabase", "false")
    .WithEnvironment("QueueName", "Standard")
    .WithEnvironment("FABRIC__HOST", "localhost")
    .WithEnvironment("FABRIC__PORT", "9494")
    .WithExternalHttpEndpoints();

// Console app that behaves like a client and runs the app integration flow.
builder.AddProject<Projects.UwvLlm_App_Core_IntegrationTest>("app-integration-test")
    .WithReference(api)
    .WaitFor(api)
    .WaitFor(llmproxy)
    .WithEnvironment("ApiBackendUrl", api.GetEndpoint("https"));

builder.Build().Run();
