# Canary — Server

Back-end REST + real-time API for **[Canary](https://almostcanary.com)**, a location-based social platform for spontaneous, real-world meetups. Members discover **Gatherings** happening near them, join in person (arrival is geofenced), capture **Snapshots** into a shared gallery, and message each other in real time. Gatherings are **ephemeral** — they decay on a fixed cadence and expire on their own, so the map only ever shows what's actually happening now.

Built with **ASP.NET Core (.NET 6)** and hosted on Azure. Internal codename *Hollow*.

> **What this project demonstrates:** a cloud-hosted backend with real-time WebSocket messaging, geospatial discovery, multi-channel notifications (push / email / SMS), a personality-based recommendation engine, background job processing, and a strict three-tier architecture built around interface seams for testability. It is a full backend system, not a demo.

---

## What it does

| Capability | Detail |
|---|---|
| **Location-based discovery** | Finds Gatherings within a geographic radius using geospatial queries (NetTopologySuite), personalised per user. |
| **Ephemeral Gatherings** | Real-world meetups with a host, guest list, group size bounds, privacy degrees, and geofenced arrival (users "arrive" only within ~75 m). A background daemon **decays** each Gathering hourly and auto-terminates it when it expires. |
| **Snapshots & feeds** | Members capture photos *inside* a Gathering into a per-Gathering gallery; a personal **Wall** feed paginates a member's Snapshots, and Snapshots can be acclaimed (rated). |
| **Affinity engine** | Each user and Gathering carries a multi-dimensional **character vector** (extraversion, athleticism, chaoticness, competitiveness, industriousness, night-owl, openness, age). Cosine similarity between vectors ranks personalised discovery, and attending a Gathering nudges a user's vector over time. |
| **Real-time messaging** | SignalR hub for 1:1 and group chat, with typing indicators, read receipts, and the ability to share Gatherings, Snapshots, and Nests directly into a conversation. |
| **Social graph** | Connections / companions, **Nests** (groups), and user blocking. |
| **Notifications** | Push via OneSignal, transactional email via SendGrid, and SMS / WhatsApp via Twilio — plus scheduled (deferred) notifications. |
| **Phone-verified accounts** | ASP.NET Core Identity with SMS-based phone verification; unverified accounts are gated out of protected actions. |
| **Trust & safety** | Reporting for Gatherings, Snapshots, and users; report-count thresholds auto-flag content; penalties/discipline can limit or lock an account. |

---

## Tech stack

| Concern | Technology |
|---|---|
| Framework | ASP.NET Core (.NET 6) |
| API style | MVC controllers, one per feature area ("Guards") over a thin operations facade |
| Real-time | SignalR (WebSockets) |
| Data access | Entity Framework Core 7 (Azure SQL / SQL Server + SQLite), NetTopologySuite for geospatial |
| Identity & auth | ASP.NET Core Identity + cookie auth, phone-number verification |
| Secrets & keys | Azure Key Vault (secrets, keys, certificates) via `Azure.Identity` |
| Cloud storage | Azure Blob Storage |
| Push notifications | OneSignal |
| Email | SendGrid |
| SMS / WhatsApp | Twilio (+ `libphonenumber-csharp` for parsing/validation) |
| Background jobs | .NET Hosted Services (`BackgroundService` daemons) |
| Image handling | System.Drawing.Common |
| Logging | Serilog (console, debug, and Azure App Service sinks) |
| API docs | Swagger / Swashbuckle |
| Testing | xUnit + coverlet across separate Core, Repository, and Frontier test projects |
| Hosting | Azure App Service |

---

## Architecture

The solution is split into three projects with a strict one-directional dependency flow, keeping business logic independent of both the web framework and the database:

```
Guard        →  Director      →  StoreCoordinator  →  EF Core / Azure SQL
(Frontier)      (Core)           (Repository)         + Blob / Key Vault
 HTTP + WS      business logic    data access
```

- **Frontier** (web tier) — MVC controllers (`*Guard`) and the SignalR hub. A Guard resolves the authenticated user, enforces phone verification, calls an *operations interface*, and shapes the HTTP response. Guards contain **no** data access.
- **Core** (domain tier) — the business logic lives in `*Director` classes behind operation interfaces, plus rich domain entities and background daemons. Core depends only on abstractions, never on EF Core or the web host.
- **Repository** (data tier) — `*StoreCoordinator` classes implement the database interfaces and own all EF Core queries, Azure Blob access, and Key Vault access.

> **A note on the naming.** The codebase uses a consistent themed vocabulary — Guards (controllers), Directors (services), Gates (domain interfaces), Coordinators (repositories), Daemons (background jobs), Manifests (inbound DTOs), Shards (outbound DTOs), and composition roots named `Ignition` / `CoreTerminal` / `Harbor`. The mapping to conventional layers is one-to-one; the theme is cosmetic, the separation is real.

### Engineering decisions worth calling out

- **Interface seams everywhere.** Each tier talks to the next through interfaces (`IGatheringOperations`, `IGatheringDatabase`, …), so Directors can be unit-tested against mocked data and mocked integrations (blob storage, push, SMS) with no database or network.
- **Single composition per tier.** `Harbor` wires every repository, `CoreTerminal` wires every Director, and `GuardBox` bundles the operation interfaces injected into controllers — dependencies are assembled once at startup in `Ignition`.
- **Environment-specific database contexts.** A base `CanaryContext` is specialised by `AzureProductionContext`, `AzureStagingContext`, and `DevelopmentContext`; the `ASPNETCORE_ENVIRONMENT` flag selects which one is wired up, with a **separate migration set per target** (Production / Staging / Test).
- **Inbound/outbound DTO split, enforced at runtime.** Requests bind to `Manifest` types; responses return `Shard` types. A guard in the response pipeline throws if a domain-only (`CoreOnlyData`) object is ever about to be serialised, so internal models can't leak to clients.
- **Ephemeral by design.** A hosted `BackgroundService` daemon runs on a fixed interval, decays every active Gathering, and terminates the ones that have expired — the lifecycle is enforced server-side, not left to clients.
- **Personalisation in the domain.** Discovery ranking is a cosine-similarity calculation over character vectors, kept as pure domain logic in `Core` rather than pushed into the query layer.
- **Secrets never live in source.** Configuration, third-party API keys, and signing material are loaded from Azure Key Vault at boot.
- **Centralised error handling.** A custom exception hierarchy (`UserErrorException` → 400, `HollowException` → 500) is mapped to structured error payloads in one place, so controllers stay focused on the happy path.
- **Soft deletes.** Entities are marked pending-deletion rather than hard-removed, so data is never dropped by accident.

### Project layout

```
Frontier/            Web tier — ASP.NET Core host
  Controllers/         MVC controllers (*Guard) + SignalR hub, grouped by feature
                       (Account, Gathering, Discover, Wall, Message, Nest, Agenda, …)
  Manifests/           Inbound request DTOs
  Services/            Third-party integrations (OneSignal, SendGrid, Twilio, sockets)
  Stores/              ASP.NET Identity user store
  Ignition.cs          Composition root: DI, auth, CORS, SignalR, Swagger, daemons

Core/                Domain tier — no web/DB dependencies
  Boundaries/          Interfaces (Gates + operation contracts) — the mockable seams
  Controls/            Business logic (*Director)
  Entities/            Rich domain models (User, Gathering, Conversation,
                       CharacterVector, …)
  Daemons/             Background services (Gathering decay/expiry)
  CoreTerminal.cs      Domain composition root

Repository/          Data tier
  Coordinators/        Data access behind interfaces (*StoreCoordinator)
  Databases/EFCore/    Contexts (env-specific), entities, migrations per target
  Storages/            Azure Blob Storage
  KeyVaults/           Azure Key Vault
  Harbor.cs            Data composition root

Core.Tests/          Domain unit tests (xUnit)
Repository.Tests/    Data-layer / integration tests (xUnit)
Frontier.Tests/      Web-layer tests (xUnit)
```

---
