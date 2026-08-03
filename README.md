# UWV LLM Integration Showcase (24-Hour PoC)

This repository contains a fast-paced, 24-hour Proof of Concept (PoC) built for a UWV application showcase. The project demonstrates how to productionize and isolate AI capabilities within an enterprise-ready architecture.

The application allows users to create accounts, send messages, and receive automated, context-aware responses powered by a local Large Language Model (LLM).

---

## 🚀 Core Showcase Features

1. Isolated LLM Auto-Reply Pipeline
- Powered by a local Ollama LLM instance.
- Completely isolated from the main Web API via an asynchronous RabbitMQ Service Bus to ensure background processing never blocks user traffic or compromises API stability.

2. Modern Cross-Platform Frontend
- Built with .NET MAUI targeting cross-platform deployment.
- Implements a clean, decoupled MVVM (Model-View-ViewModel) architecture for optimal state management and UI responsiveness.

3. Structural Flex: Advanced Architecture with gAPI
- To eliminate boilerplate and accelerate development within the 24-hour limit, the project uses gAPI (including a custom lightweight RabbitMQ module).
- No manual REST endpoints: Client-to-server calls ([GenerateApi]) and server-to-client real-time notifications via SSE ([GenerateHub]) are fully generated from shared C# interfaces.

--- 

## 🏗️ System Architecture
The data flows asynchronously through isolated microservices to guarantee scalability:


```
MAUI App (MVVM) --(gAPI REST)--> Web API --(RabbitMQ Bus)--> LLM Proxy --(REST)--> Ollama
                                                                                     |
MAUI App (SSE)  <--(gAPI SSE)--- Web API <--(RabbitMQ Bus)-- LLM Proxy <-------------+
```

---

## 🚦 Project Status & Progress

The core end-to-end backend pipeline was successfully designed and built within the 24-hour time constraint.
- [x] MAUI Frontend & MVVM: User registration, authentication, and message dispatching are fully functional.
- [x] gAPI Client-to-Server: Automated API generation handles all frontend-to-backend REST communication flawlessly.
- [x] Service Bus Isolation: Web API strictly handshakes with RabbitMQ; LlmProxy processes AI tasks asynchronously.
- [x] Ollama Integration: The local LLM generates context-aware replies and stores them back into the DB via the generated CRUD infrastructure.
- [ ] Real-time gAPI SSE Callback: In Progress. The backend triggers the response handler, but the real-time gAPI Server-Sent Events (SSE) plumbing inside the .NET MAUI client is currently being ironed out.

---

## 📂 Repository Structure

The codebase is highly modularized, ensuring strict separation of concerns, testability, and automated scaffolding:

### 📱 Frontend (App)

- \UwvLlm.App: .NET MAUI frontend housing XAML pages and platform-specific services.
- \UwvLlm.App.Core: Core frontend business logic, interfaces, and MVVM ViewModels.
- \UwvLlm.App.Core.Test & .IntegrationTest: Unit tests and full data-flow integration tests.

### ⚙️ Backend (Api)

- \UwvLlm.Api: ASP.NET Core Web API host and startup configurations.
- \UwvLlm.Api.Core: Core backend services and RabbitMQ service bus handlers.
- \UwvLlm.Api.Core.Test: Backend unit tests.

### 🤖 AI Processing (LLM Proxy)

- \UwvLlm.LlmProxy & .Core: Isolated console host and handlers translating bus messages into AI tasks.
- \UwvLlm.LlmProxy.Core.Test: Unit tests for proxy behavior.

### 📦 Infrastructure & Shared Layers

- \UwvLlm.Shared.Public: Publicly shared interfaces, DTOs, and enums fueling the gAPI generation engine.
- \UwvLlm.Shared.Private: Internal-only contracts, specifically for service bus communication.
- \UwvLlm.Infrastructure.Data: Entity Framework Core context, migrations, mappings, and table-specific permission/query UseCases.
- \UwvLlm.Infrastructure.Llm: Low-level integration wrappers, clients (Ollama/ChatGPT), models, and enums for AI communication.

### 🛠️ Ecosystem Tools & Orchestration

- \UwvLlm.AppHost: .NET Aspire orchestration host to spin up the entire multi-project ecosystem effortlessly.
- \UwvLlm.BackendGenerator: Automated CLI tool generating backend services, mappers, UseCases, and shared contracts directly from the EF model.
- \UwvLlm.Fabric: Custom gAPI backplane managing real-time Server-Sent Events (SSE) and WebSockets (WSS).
- \UwvLlm.Storage: Dedicated physical storage Web API infrastructure.

---

## 🛠️ Development Guidelines

- **Preserve Isolation:** Keep AI orchestrations strictly inside LlmProxy and Infrastructure.Llm. Never leak LLM direct dependencies into the Web API or Frontend.
- **Contracts First:** Public API contracts belong in Shared.Public. Internal bus messages belong in Shared.Private.
- **Model-Driven CRUD:** If a data service needs changes, update the EF model or the BackendGenerator. Never manually edit generated code.