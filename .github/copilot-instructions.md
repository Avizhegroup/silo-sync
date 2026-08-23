# Copilot Instructions — Silo (WMS)

## Project Overview

Silo is a large-scale enterprise **Warehouse Management System (WMS)** using RFID, built on **.NET 9**. It is a multi-project solution organized around **Clean Architecture** with strict domain-based foldering.

**Solution file:** `Silo.slnx`  
**All projects target:** `net9.0` (set in `Directory.build.props`)

---

## Architecture

### Layer Structure

```
Silo.Domains              → Domain entities and EF Core DbContext (WmsApiContext)
Silo.Application          → CQRS handlers, commands, queries, view models (MediatR)
Silo.Application.Api      → Application-level API contracts
Silo.Api                  → ASP.NET Core Web API (versioned: v1/v2/v3)
Silo.Infrastructure.Web   → HttpClient (RfidConnectApi), caching, auth, export utilities
Silo.Infrastructure.Shared → Shared infrastructure (logging via Serilog, etc.)
Silo.Identity.Client      → Blazor auth/claims (SiloBasePage, SiloAuthenticationStateProvider)
Silo.Identity.Server      → JWT identity services
Silo.Shared               → TextResources (localization .resx), shared utilities
Silo.Shared.Components    → Reusable Blazor components (Modal, etc.)
Silo.Ui.Customer          → Blazor Server UI (customer-facing)
Silo.Ui.Gate              → Blazor Server UI (gate operations)
Silo.Ui.Bypass            → Blazor Server UI (bypass flow)
Silo.Modules.*            → Feature modules (Ai, Document, Guarantee, Inspect, Product, TruckCross)
Silo.Jobs.Win             → Windows service jobs
Silo.Ai.Agent             → AI chat agent (chatbot instructions in Chat/*.md)
```

### Domain-Based Organization (Critical)

There is a strict **1:1:1 mapping**:

```
Silo.Domains\Entities\{Domain}.cs
    → Silo.Application\Features\{Domain}\Commands\ and \Queries\
    → Silo.Api\Controllers\v2\{Domain}Controller.cs
```

**Always check `Silo.Domains\Entities\` first** to identify the correct domain name before creating any CQRS feature, controller, or folder.

---

## CQRS Pattern (MediatR)

All new features in `Silo.Application\Features\` follow this structure per domain:

```
Features\{DomainName}\
├── Commands\
│   └── {Verb}{DomainName}\
│       ├── {Verb}{DomainName}Command.cs
│       ├── {Verb}{DomainName}Handler.cs
│       └── {Verb}{DomainName}Vm.cs
└── Queries\
    └── {Get|Check}{Description}\
        ├── {QueryName}Query.cs
        ├── {QueryName}Handler.cs
        └── {QueryName}Vm.cs
```

**Naming conventions:**
- Command: `{Verb}{EntityName}Command` — e.g. `CreateNewActionTypeCommand`
- Query: `{Get|Check}{Description}Query` — e.g. `GetAllActionTypesQuery`
- Handler: `{CommandOrQueryName}Handler`
- ViewModel: `{CommandOrQueryName}Vm`
- DTO: `{CommandOrQueryName}Dto`

**All handlers and commands use `namespace Silo.Application.Features;` (flat, file-scoped).**

### Handler patterns

- Use **primary constructors** for DI: `public class MyHandler(WmsApiContext context, IMapper mapper)`
- Inject `WmsApiContext` for EF Core; `IDataAccess` for raw SQL
- Pass `cancellationToken` to all async EF Core calls
- Return `null` for not-found (do not throw); handle null in the controller
- Prefer `.Select()` projections over `.Include()` + AutoMapper for read-heavy queries

### V2 Controller pattern

```csharp
public class ActionTypeController(ILogger<ActionTypeController> logger, IMediator mediator)
    : SiloBaseControllerVersion2(logger)
{
    [HttpPost("[action]")]
    public async Task<IActionResult> Create(CreateNewActionTypeCommand command)
        => Ok(new ApiResponse() { Successful = true, Value = await mediator.Send<CreateNewActionTypeVm>(command) });
}
```

API route base: `RfidCore/v{version:apiversion}/[controller]`

---

## View Models & JSON Serialization

Every VM used in an API response needs a **JsonSerializerContext** (source generators):

```csharp
[JsonSerializable(typeof(ApiResponse<List<GetAllActionTypesVm>>))]
public partial class GetAllActionTypesVmContext : JsonSerializerContext { }
```

- Place context classes in the same file as the VM
- Skip contexts for primitives (`int`, `bool`, `string`)
- Use `ApiResponse<T>` wrapper for all typed responses

---

## Blazor UI Conventions

- **No `@code` blocks** in `.razor` files — always use a `.razor.cs` code-behind partial class
- Use `[Inject]` attribute for DI in components (not constructor injection)
- All Blazor pages `@inherits SiloBasePage` (from `Silo.Identity.Client`)
- Event handler naming: `On[Name][EventName]` — e.g. `OnClearClick`, `OnSaveSubmit`
- Use `RfidConnectApi` for all HTTP calls from Blazor (`Silo.Infrastructure.Web.HttpClient`)
- Always show a `TelerikLoaderContainer` (`IsLoading` state) during API calls
- Use **`TelerikGrid`** for all data list views (sorting, paging, export)
- Add shared usings to `GlobalUsings.cs`; add Blazor-specific imports to `_Imports.razor`

### RfidConnectApi call pattern

```csharp
var result = (await Api.PostAsyncByContext<List<GetAllWarehousesVm>>(
    "StoredProcedureName", new GetAllWarehousesVmContext())).Value;
```

---

## Code Style (enforced via `.editorconfig`)

- **File-scoped namespaces** are **required** (`csharp_style_namespace_declarations = file_scoped:error`)
- `using` directives go **outside** the namespace
- Unused usings are errors (`IDE0005.severity = error`)
- Unused local variables and unread fields are errors
- `charset = utf-8-bom` for all `.cs` files
- Allman brace style (`csharp_new_line_before_open_brace = all`)
- No `this.` qualifiers

---

## Localization

All user-visible strings come from `Silo.Shared\TextResources.resx` via `TextResources` static class. Use:

```csharp
[Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Title))]
```

Never hardcode UI strings.

---

## Existing Prompt Files

The `.github/prompts/` folder contains reusable prompt files for common tasks:
- `cqrs-implementation.prompt.md` — full CQRS scaffold guide with examples
- `refactoring-blazor.prompt.md` — Blazor component refactoring rules
- `dynamic-report.prompt.md` — dynamic report builder pattern
- `sql-query-tune.prompt.md` — SQL optimization rules (use CTEs, avoid SELECT *, no scenario changes)
