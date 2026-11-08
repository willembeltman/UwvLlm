var builder = DistributedApplication.CreateBuilder(args);

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
    .WithReference(rabbit)
    .WithReference(storage)
    .WithReference(fabric)
    .WithReference(llmproxy)
    .WaitFor(rabbit)
    .WithEnvironment("CreateDatabase", "false")
    .WithEnvironment("UseMemoryDatabase", "false")
    .WithEnvironment("QueueName", "Standard")
    .WithEnvironment("FABRIC__HOST", "localhost")
    .WithEnvironment("FABRIC__PORT", "9494")
    .WithExternalHttpEndpoints();

builder.Build().Run();
