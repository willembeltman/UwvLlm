

# Global project structure

## App

	\UwvLlm.App
		- Maui app: Frontend
	\UwvLlm.App\Pages
		- De xaml pages
	\UwvLlm.App\Services
		- App specifieke service implementaties voor frontend services

	\UwvLlm.App.Core
		- Library: Frontend
	\UwvLlm.App.Core\Interfaces
		- Frontend service interfaces
	\UwvLlm.App.Core\Services
		- Frontend service implementaties
	\UwvLlm.App.Core\ViewModels
		- Frontend view models

	\UwvLlm.App.Core.IntegrationTest
		- Console app: frontend app test data flow test

	\UwvLlm.App.Core.Test
		- Unit tests: Frontend

## Api

	\UwvLlm.Api 
		- WebApi: Backend
	\UwvLlm.Api\Extensions
		- Startup extentions

	\UwvLlm.Api.Core 
		- Library: Backend
	\UwvLlm.Api.Core\Handlers
		- Service bus handlers
	\UwvLlm.Api.Core\Services
		- Service implementaties voor backend services

	\UwvLlm.Api.Core.Test
		- Unit tests: Backend

## Llm proxy

	\UwvLlm.LlmProxy
		- Console app: Servicebus endpoint Llm proxy
	\UwvLlm.LlmProxy\Extensions
		- Voor startup

	\UwvLlm.LlmProxy.Core
		- Library: Llm proxy
	\UwvLlm.LlmProxy.Core\Handlers
		- Service bus handlers

	\UwvLlm.LlmProxy.Core.Test
		- Unit tests: Llm proxy

## Shared

	\UwvLlm.Shared.Private
		- Library: shared interfaces / dtos voor backend projecten
	\UwvLlm.Shared.Private\Messages
		- De service bus messages

	\UwvLlm.Shared.Public
		- Library: shared interfaces / dtos voor alle projecten
	\UwvLlm.Shared.Public\CrudInterfaces
		- Publiek gedeelde interfaces specifiek voor de crud services
	\UwvLlm.Shared.Public\Dtos
		- Publiek gedeelde dtos gebruikt in de interfaces
	\UwvLlm.Shared.Public\Enums
		- Public gedeelde enums
	\UwvLlm.Shared.Public\Interfaces
		- Public gedeelde interfaces

	\UwvLlm.Infrastructure.Data
		- Library: EF context/entities
	\UwvLlm.Infrastructure.Data\CrudServices
		- Publieke interfaces voor data (ook wel eens repositories genoemd)
	\UwvLlm.Infrastructure.Data\Entities
		- EF entities / ApplicationDbContext
	\UwvLlm.Infrastructure.Data\Mappings
		- Mappings / projections tussen entities en dtos
	\UwvLlm.Infrastructure.Data\Migrations
		- EF migrations
	\UwvLlm.Infrastructure.Data\UseCases
		- Een tabel specifieke connector: ef <-rechten/queries-> crud/dtos

	\UwvLlm.Infrastructure.Llm
		- Library: Ollama communicatie
	\UwvLlm.Infrastructure.Llm\Clients
		- Clients voor Ollama en ChatGPT
	\UwvLlm.Infrastructure.Llm\Enums
		- Enums specifiek voor Llm communicatie
	\UwvLlm.Infrastructure.Llm\Interfaces
		- Interfaces specifiek voor Llm communicatie
	\UwvLlm.Infrastructure.Llm\Models
		- Models specifiek voor Llm communicatie

## Tools

	\UwvLlm.BackendGenerator
		- Console app: gAPI generator, genereerd backend services (service, mapper en usecase), shared service intefaces en shared dtos

	\UwvLlm.Fabric
		- Console app: gAPI backplane voor Sse en Wss

	\UwvLlm.Storage
		- WebApi: Storage server, fysiek storage project (one time install)

	\UwvLlm.AppHost
		- Microsoft Aspire AppHost
