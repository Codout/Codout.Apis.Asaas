# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unofficial .NET SDK (NuGet package ID: `Asaas.Api`) for integrating with the [Asaas](https://www.asaas.com) payment platform. Targets **.NET 10.0**.

## Build Commands

```bash
dotnet build Codout.Apis.Asaas.sln       # Build entire solution
dotnet build Codout.Apis.Asaas/           # Build library only
dotnet test Codout.Apis.Asaas.Tests/      # Run all 400 unit tests
dotnet test Codout.Apis.Asaas.Tests/ --filter "FullyQualifiedName~CustomerManager"  # Run single test class
dotnet run --project Codout.Apis.Asaas.Sample/  # Run sample console app
dotnet pack Codout.Apis.Asaas/            # Create NuGet package
```

Note: The Sample project has a broken reference to `AsaasClient` — always build the library with `dotnet build Codout.Apis.Asaas/` instead of the full solution if the sample fails.

## Architecture

**Solution has three projects:**
- `Codout.Apis.Asaas` — The SDK library
- `Codout.Apis.Asaas.Tests` — xUnit test project (400 tests, Moq for mocking)
- `Codout.Apis.Asaas.Sample` — Console app demonstrating usage

**Key architectural pattern — Manager/Facade:**

1. **`AsaasApi`** (`AsaasApi.cs`) — Top-level facade. Exposes 20 domain managers via `Lazy<T>` properties. This is the single entry point consumers use.

2. **`BaseManager`** (`Core/BaseManager.cs`) — Abstract base for all managers. Handles HTTP communication (GET/POST/PUT/DELETE), authentication (`access_token` header), JSON serialization, multipart file uploads, and response parsing. All API calls go through `/v3` routes. Uses `System.Text.Json` exclusively. `BuildHttpClient()` is `protected virtual` to enable test mocking.

3. **Domain Managers** (`Managers/`) — Each maps to an Asaas API domain (Customer, Payment, Subscription, etc.). They inherit `BaseManager` and expose typed CRUD operations.

4. **Models** (`Models/`) — Organized by domain. Each domain folder contains request models (`Create*Request`, `Update*Request`, `Filter*`), response models, and enums. JSON property names use `[JsonPropertyName]` (System.Text.Json) attributes when the API name differs from the C# property name.

5. **Response types** (`Core/Response/`) — `ResponseObject<T>` for single items, `ResponseList<T>` for paginated lists. Both extend `BaseResponse` which parses errors and exposes `WasSucessfull()` (note: existing typo in codebase).

**Environments:** `AsaasEnvironment.PRODUCTION` → `https://api.asaas.com`, `AsaasEnvironment.SANDBOX` → `https://api-sandbox.asaas.com`

## Conventions

- Serialization config is in `Core/JsonSerializerConfiguration.cs` — camelCase naming, null-ignoring, enum-as-string.
- Query parameters for list/filter operations are built via `RequestParameters` (`Core/RequestParameters.cs`), a dictionary wrapper that generates query strings.
- Extension methods live in `Core/Extension/` (DateTime formatting, status code checks, string helpers).
- No external dependencies — uses only `System.Text.Json` (built-in).

## Testing

Tests use xUnit + Moq. Each manager has a testable subclass (`Testable*Manager` in `Tests/Helpers/TestableManagerFactory.cs`) that overrides `BuildHttpClient()` to inject a `MockHttpMessageHandler`. Test classes inherit from `ManagerTestBase<T>` which provides helpers: `SetupOkResponse()`, `SetupListResponse()`, `SetupErrorResponse()`, `AssertRequestMethod()`, `AssertRequestUrl()`, `AssertRequestUrlContains()`.

Known limitation: `BaseDeleted` and `BaseTransfer` are abstract — `System.Text.Json` cannot deserialize them. Tests for delete/transfer operations use error responses or empty arrays to avoid this.
