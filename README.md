# Infokom.CMS.LeadResponder

A small ASP.NET Core Web API that answers customer chat messages using configurable business context and a pluggable AI provider. It runs out of the box with a mock AI provider and in-memory storage — no credentials needed.

## Structure

- `src/Infokom.CMS.LeadResponder.Domain` – models (`Conversation`, `ChatMessage`, `BusinessContext`)
- `src/Infokom.CMS.LeadResponder.Application` – chat flow, validation, `IAiProvider` / `IConversationStore` interfaces
- `src/Infokom.CMS.LeadResponder.Infrastructure` – mock AI provider, in-memory store, DI registration
- `src/Infokom.CMS.LeadResponder.Api` – HTTP endpoints
- `tests/Infokom.CMS.LeadResponder.Application.Tests` – unit tests

## Run

Requires the .NET 10 SDK.

```bash
dotnet test
dotnet run --project src/Infokom.CMS.LeadResponder.Api
```

## Endpoints

- `GET /health` – health check
- `POST /api/chat` – body `{ "conversationId": "optional", "message": "Hello" }`, returns `{ "conversationId": "...", "reply": "..." }`.
  Invalid input returns `400` (problem details); unexpected errors return `500` (problem details).

```bash
curl -X POST http://localhost:5000/api/chat -H 'Content-Type: application/json' -d '{"message":"Hi"}'
```

## Configuration

Business context lives in `appsettings.json` under `BusinessContext` (`BusinessName`, `Description`).

## Configuring a real AI provider later

1. Implement `IAiProvider` in the Infrastructure project (e.g. `OpenAiProvider`).
2. Register it in `DependencyInjection.AddLeadResponder` in place of `MockAiProvider`.
3. Supply secrets (API keys, endpoints) outside source control — never commit them:
   - local development: `dotnet user-secrets set "AiProvider:ApiKey" "<key>" --project src/Infokom.CMS.LeadResponder.Api`
   - production: environment variables (e.g. `AiProvider__ApiKey`) or a secret store.

Conversation storage can be replaced the same way by implementing `IConversationStore`.
