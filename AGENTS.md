# UwvLlm project notes for AI agents

This repository is a one-week proof of concept built for a UWV application showcase. It is intentionally not a finished production system. When working in this repo, preserve the architecture unless the user explicitly asks for a redesign.

## Goal of the project

The PoC demonstrates an app where users can create an account, send messages to each other, and have incoming messages automatically answered by an LLM.

The main flow is:

```text
app --generated REST/gAPI--> api --service bus-->
llmproxy --REST--> ollama --REST-->
llmproxy --service bus--> api --generated SSE/gAPI--> app
```

## What gAPI is

gAPI is the central showcase technology in this project. Think of it as similar in spirit to gRPC, but using C# interfaces instead of protobuf files.

Developers define shared interfaces and mark them with gAPI attributes. gAPI then generates the client/server plumbing so application code can use dependency-injected interfaces as if the implementation were local, while the implementation actually runs elsewhere. Authentication, state handling, generated REST calls, and generated SSE callbacks are handled by the gAPI infrastructure.

Do not propose replacing or modifying the gAPI library as a first solution. In this repository, gAPI usage is the point of the showcase.

## Client to server APIs

For client-to-server calls:

- The shared interface lives in `UwvLlm.Shared.Public`.
- The interface is marked with `[GenerateApi]`.
- The server-side implementation lives in an API/core project.
- The client injects and calls the interface directly.

Example:

- Interface: `UwvLlm.Shared.Public/Interfaces/IMailApi.cs`
- Implementation: `UwvLlm.Api.Core/Services/MailApi.cs`
- Client usage: `UwvLlm.App.Core/Services/EmailService.cs`

`IMailApi` is marked with `[GenerateApi]` and `[IsAuthorized]`. The app injects `IMailApi` and calls `SendMail`. The actual implementation runs in `MailApi` on the API side.

API startup is configured in `UwvLlm.Api/Program.cs` with:

- `AddAutoApi(...)`
- `MapAutoApi<AuthenticationMiddleware<...>>()`
- authentication/state services and state mappings

Client startup is configured in `UwvLlm.App/MauiProgram.cs` with:

- `AddAutoApiClient()`
- `AddAuthenticationServices<State>(...)`

## Server to client hubs

For server-to-client callbacks:

- The shared interface lives in `UwvLlm.Shared.Public`.
- The interface is marked with `[GenerateHub]`.
- The client-side implementation implements that interface and subscribes through `IClientConnection`.
- The server injects the generated hub context and calls clients through it.

Example:

- Interface: `UwvLlm.Shared.Public/Interfaces/INotificationHub.cs`
- Client implementation: `UwvLlm.App.Core/ViewModels/NotificationHubViewModel.cs`
- Server-side caller: `UwvLlm.Api.Core/Handlers/GenerateAutoReplyResponseHandler.cs`

`INotificationHub` is marked with `[GenerateHub]` and `[IsAuthorized]`. `NotificationHubViewModel` implements the interface and subscribes with `ClientConnection.SubscribeAsync(this)`. The API side uses `INotificationHubContext`, for example `notificationHub.ToAll.OnNotificationReceived(...)`, to send generated SSE callbacks back to the app.

SSE startup is configured in:

- API: `AddAutoSse(...)` and `MapAutoSse()` in `UwvLlm.Api/Program.cs`
- App: `AddAutoSseClient()` in `UwvLlm.App/MauiProgram.cs`

## Service bus and LLM proxy

RabbitMQ is used as a service bus between the API and the LLM proxy. This is a quick generic solution built for the PoC.

The relevant pieces are:

- API caller: `UwvLlm.Api.Core/Services/MailApi.cs`
- LLM proxy handler: `UwvLlm.LlmProxy.Core/Handlers/GenerateAutoReplyRequestHandler.cs`
- API response handler: `UwvLlm.Api.Core/Handlers/GenerateAutoReplyResponseHandler.cs`
- Message contracts: `UwvLlm.Shared.Private/Messages`

When mail is sent, `MailApi.SendMail` stores the message through the generated CRUD service and sends a `GenerateAutoReplyRequest` to the `"LlmProxy"` bus. The LLM proxy receives that request, calls Ollama through `ILlmClient`/`OllamaClient`, stores the generated auto-response, and sends a `GenerateAutoReplyResponse` back to the `"Api"` bus. The API then creates a notification and sends it to clients through the generated gAPI hub.

Service bus startup is configured in:

- API: `UwvLlm.Api/Program.cs`, ending with `app.RunWithServiceBus(busName: "Api")`
- LLM proxy: `UwvLlm.LlmProxy/Program.cs`, ending with `StartConsoleWithServiceBusAsync()`

## Backend generator

The project includes `UwvLlm.BackendGenerator`, which generates CRUD services and infrastructure from the Entity Framework data model.

Generator configuration is in `UwvLlm.BackendGenerator/Program.cs`. It points generation output to:

- DTOs: `UwvLlm.Shared.Public/Dtos`
- CRUD interfaces: `UwvLlm.Shared.Public/CrudInterfaces`
- CRUD use cases/services/mappings: `UwvLlm.Infrastructure.Data`
- API extension wiring: `UwvLlm.Api/Extensions`

Generated CRUD services are part of normal app flow. For example, `MailApi` uses `IMailMessagesCrudService`, and notification handling uses `IUserNotificationsCrudService`.

## Practical guidance

- Prefer using the existing generated gAPI interfaces and CRUD services instead of adding hand-written REST endpoints.
- Keep public API contracts in `UwvLlm.Shared.Public`.
- Keep internal service bus message contracts in `UwvLlm.Shared.Private`.
- Keep API-side business behavior in `UwvLlm.Api.Core`.
- Keep MAUI app view models and app-facing services in `UwvLlm.App.Core`.
- Keep Ollama/LLM proxy behavior in `UwvLlm.LlmProxy.Core` and `UwvLlm.Infrastructure.Llm`.
- If a generated file or generated directory looks wrong, first inspect `UwvLlm.BackendGenerator/Program.cs` and the EF model before manually editing generated output.
