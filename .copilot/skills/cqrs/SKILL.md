---
name: cqrs
description: "Scaffold CQRS features (Commands, Queries, Handlers, ViewModels, Controllers) in the Silo WMS project following the strict domain-based 1:1:1 pattern. Use when adding new features, creating API endpoints, or migrating legacy WmsBusiness code."
---

# /cqrs

Implement CQRS features in the Silo project using MediatR, EF Core, and AutoMapper.

## Usage

```
/cqrs create <DomainName> <OperationName>    # scaffold a full Command + Handler + VM + Controller action
/cqrs query <DomainName> <QueryName>         # scaffold a Query + Handler + VM + Controller action
/cqrs migrate <WmsBusinessMethod>            # migrate a legacy WmsBusiness method to CQRS
/cqrs checklist                              # print the implementation checklist
```

If invoked with no arguments and a user description (e.g. `/cqrs I need to save a Product`), infer the domain and operation type automatically, then scaffold all required files.

---

## What You Must Do When Invoked

1. **Identify the domain entity** — check `Silo.Domains\Entities\` for the matching entity. The domain name is that entity's class name (e.g. `Product`, `ActionType`, `DocumentHeader`).
2. **Determine operation type** — writes = Command, reads = Query.
3. **Create all required files** listed in the checklist below.
4. **Never put all domains in a shared folder** — each domain gets its own `Features\{DomainName}\` folder.
5. **Always use `namespace Silo.Application.Features;`** (flat, file-scoped) for all CQRS files.

---

## Strict 1:1:1 Mapping Rule

```
Silo.Domains\Entities\{Domain}.cs
    ↓
Silo.Application\Features\{Domain}\Commands\{CommandName}\   (or Queries\)
    ↓
Silo.Api\Controllers\v2\{Domain}Controller.cs
```

---

## Implementation Checklist

For each new feature:

- [ ] Find entity in `Silo.Domains\Entities\{DomainName}.cs`
- [ ] Create `Silo.Application\Features\{DomainName}\Commands\{Name}\` or `Queries\{Name}\`
- [ ] `{Name}Command.cs` or `{Name}Query.cs`
- [ ] `{Name}Handler.cs`
- [ ] `{Name}Vm.cs` (with `JsonSerializerContext` if used in API response)
- [ ] Add action to `Silo.Api\Controllers\v2\{DomainName}Controller.cs` (create if missing)
- [ ] Add AutoMapper profile in `Silo.Application\Profiles\` if mapping is needed

---

## File Templates

### Command

```csharp
namespace Silo.Application.Features;

public class {Name}Command : IRequest<{Name}Vm>
{
    public int? Id { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Title))]
    [Required(ErrorMessageResourceType = typeof(TextResources),
              ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string? Title { get; set; }
}
```

### Query

```csharp
namespace Silo.Application.Features;

public class {Name}Query : IRequest<{Name}Vm>
{
    public int Id { get; set; }
}
// For lists: IRequest<List<{Name}Vm>>
```

### Command Handler

```csharp
namespace Silo.Application.Features;

public class {Name}Handler(WmsApiContext context, IMapper mapper)
    : IRequestHandler<{Name}Command, {Name}Vm>
{
    public async Task<{Name}Vm> Handle({Name}Command request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<{Entity}>(request);

        if (request.Id.HasNoValue())
        {
            await context.{Entities}.AddAsync(entity, cancellationToken);
        }
        else
        {
            context.{Entities}.Update(entity);
        }

        var result = await context.SaveChangesAsync(cancellationToken) > 0;
        return new() { Result = result };
    }
}
```

### Query Handler (projection — preferred for reads)

```csharp
namespace Silo.Application.Features;

public class {Name}Handler(WmsApiContext context)
    : IRequestHandler<{Name}Query, List<{Name}Vm>>
{
    public async Task<List<{Name}Vm>> Handle({Name}Query request, CancellationToken cancellationToken)
    {
        return await context.{Entities}
            .Select(x => new {Name}Vm
            {
                Id = x.Id,
                Title = x.Title
            })
            .ToListAsync(cancellationToken);
    }
}
```

### ViewModel with JsonSerializerContext

```csharp
namespace Silo.Application.Features;

public class {Name}Vm
{
    public bool Result { get; set; }
    public int Id { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Title))]
    public string? Title { get; set; }
}

[JsonSerializable(typeof(ApiResponse<{Name}Vm>))]
public partial class {Name}VmContext : JsonSerializerContext { }
// For lists: ApiResponse<List<{Name}Vm>>
// Skip context for primitives (int, bool, string)
```

### V2 Controller

```csharp
namespace Silo.Api.Controllers.v2;

public class {Domain}Controller(ILogger<{Domain}Controller> logger, IMediator mediator)
    : SiloBaseControllerVersion2(logger)
{
    [HttpPost("[action]")]
    public async Task<IActionResult> Create({Name}Command command)
        => Ok(new ApiResponse() { Successful = true, Value = await mediator.Send<{Name}Vm>(command) });

    [HttpGet("[action]")]
    public async Task<IActionResult> ReadAll()
        => Ok(new ApiResponse() { Successful = true, Value = await mediator.Send<List<{Name}Vm>>(new {Name}Query()) });

    [HttpDelete("[action]")]
    public async Task<IActionResult> Delete(Delete{Domain}ByIdCommand command)
        => Ok(new ApiResponse() { Successful = true, Value = await mediator.Send<Delete{Domain}ByIdVm>(command) });
}
```

---

## Handler Patterns

### Delete (use ExecuteDeleteAsync — no entity loading)

```csharp
var rowsAffected = await context.Entities
    .Where(x => x.Id == request.Id)
    .ExecuteDeleteAsync(cancellationToken);
return new() { Result = rowsAffected > 0 };
```

### Bulk Update (use ExecuteUpdateAsync)

```csharp
var rowsAffected = await context.Entities
    .Where(x => request.Ids.Contains(x.Id))
    .ExecuteUpdateAsync(setters => setters
        .SetProperty(e => e.Status, request.NewStatus), cancellationToken);
```

### Transaction (only use try-catch when using transactions)

```csharp
using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
try
{
    // multiple saves...
    await transaction.CommitAsync(cancellationToken);
    return new() { Result = true };
}
catch (Exception ex)
{
    await transaction.RollbackAsync(cancellationToken);
    logger.LogError(ex, "Transaction failed");
    throw;
}
```

**🚫 Do NOT use try-catch in simple CRUD handlers — let exceptions bubble up to the global handler.**

---

## AutoMapper Profile

```csharp
// Location: Silo.Application\Profiles\{DomainName}\{DomainName}Profile.cs
namespace Silo.Application.Features;

public class {Domain}Profile : Profile
{
    public {Domain}Profile()
    {
        CreateMap<{Name}Command, {Entity}>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<{Entity}, {Name}Vm>();
    }
}
```

Register in `Silo.Api\Program.Services.cs` under `services.AddAutoMapper(...)`.

---

## Common Handler Dependencies

| Service | Purpose |
|---------|---------|
| `WmsApiContext` | EF Core database context (always needed) |
| `IMapper` | AutoMapper mappings |
| `IDataAccess` | Raw T-SQL execution |
| `ILogger<T>` | Logging |
| `IHttpContextAccessor` | User claims/context |

---

## Migration from Legacy WmsBusiness

When refactoring an existing `WmsBusiness` method:

1. Identify: is it a read (Query) or write (Command)?
2. Create folder under `Features\{DomainName}\`
3. Define Command/Query from the method parameters
4. Move business logic into the Handler
5. Create VM from return type
6. Add `JsonSerializerContext` for the VM
7. Add controller action using `mediator.Send()`
8. Remove old Business method once API is wired up
