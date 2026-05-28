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

# Global project structure

## App

    \UwvLlm.App
        - MAUI app: Frontend
    \UwvLlm.App\Pages
        - The XAML pages
    \UwvLlm.App\Services
        - App-specific service implementations for frontend services

    \UwvLlm.App.Core
        - Library: Frontend
    \UwvLlm.App.Core\Interfaces
        - Frontend service interfaces
    \UwvLlm.App.Core\Services
        - Frontend service implementations
    \UwvLlm.App.Core\ViewModels
        - Frontend view models

    \UwvLlm.App.Core.IntegrationTest
        - Console app: frontend app data flow integration test

    \UwvLlm.App.Core.Test
        - Unit tests: Frontend

## Api

    \UwvLlm.Api
        - WebApi: Backend
    \UwvLlm.Api\Extensions
        - Startup extensions

    \UwvLlm.Api.Core
        - Library: Backend
    \UwvLlm.Api.Core\Handlers
        - Service bus handlers
    \UwvLlm.Api.Core\Services
        - Service implementations for backend services

    \UwvLlm.Api.Core.Test
        - Unit tests: Backend

## LLM proxy

    \UwvLlm.LlmProxy
        - Console app: Service bus endpoint LLM proxy
    \UwvLlm.LlmProxy\Extensions
        - Startup extensions

    \UwvLlm.LlmProxy.Core
        - Library: LLM proxy
    \UwvLlm.LlmProxy.Core\Handlers
        - Service bus handlers

    \UwvLlm.LlmProxy.Core.Test
        - Unit tests: LLM proxy

## Shared

    \UwvLlm.Shared.Private
        - Library: shared interfaces / DTOs for backend projects
    \UwvLlm.Shared.Private\Messages
        - Service bus messages

    \UwvLlm.Shared.Public
        - Library: shared interfaces / DTOs for all projects
    \UwvLlm.Shared.Public\CrudInterfaces
        - Public shared interfaces specifically for CRUD services
    \UwvLlm.Shared.Public\Dtos
        - Public shared DTOs used in the interfaces
    \UwvLlm.Shared.Public\Enums
        - Public shared enums
    \UwvLlm.Shared.Public\Interfaces
        - Public shared interfaces

    \UwvLlm.Infrastructure.Data
        - Library: EF context/entities
    \UwvLlm.Infrastructure.Data\CrudServices
        - Public data interfaces (sometimes referred to as repositories)
    \UwvLlm.Infrastructure.Data\Entities
        - EF entities / ApplicationDbContext
    \UwvLlm.Infrastructure.Data\Mappings
        - Mappings / projections between entities and DTOs
    \UwvLlm.Infrastructure.Data\Migrations
        - EF migrations
    \UwvLlm.Infrastructure.Data\UseCases
        - Table-specific connector: EF <-permissions/queries-> CRUD/DTOs

    \UwvLlm.Infrastructure.Llm
        - Library: Ollama communication
    \UwvLlm.Infrastructure.Llm\Clients
        - Clients for Ollama and ChatGPT
    \UwvLlm.Infrastructure.Llm\Enums
        - Enums specific to LLM communication
    \UwvLlm.Infrastructure.Llm\Interfaces
        - Interfaces specific to LLM communication
    \UwvLlm.Infrastructure.Llm\Models
        - Models specific to LLM communication

## Tools

    \UwvLlm.BackendGenerator
        - Console app: gAPI generator, generates backend services (service, mapper, and use case), shared service interfaces, and shared DTOs

    \UwvLlm.Fabric
        - Console app: gAPI backplane for SSE and WSS

    \UwvLlm.Storage
        - WebApi: Storage server, physical storage project (one-time install)

    \UwvLlm.AppHost
        - Microsoft Aspire AppHost