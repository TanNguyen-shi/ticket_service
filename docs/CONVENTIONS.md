# Code Conventions & Patterns

---

## Naming Conventions

### Solution & Projects
- **Solution**: `TicketingSystem.sln`
- **Projects**: PascalCase
  - `TicketingSystem` — Web API / Presentation
  - `Ticketing.Application` — Use Cases
  - `Ticketing.Domain` — Domain Services
  - `Ticketing.Infrastructure` — Data Access & Utilities

### Namespaces
```csharp
// Controllers
TicketingSystem.Controllers.Admin.Event
TicketingSystem.Controllers.Client

// Use Cases
Ticketing.Application.UseCases.Admin.Event.Interfaces
Ticketing.Application.UseCases.Admin.Event

// Domain Services
Ticketing.Domain.Domain.Event.Interfaces
Ticketing.Domain.Domain.Event

// Repositories
Ticketing.Infrastructure.Repositories.Event
Ticketing.Infrastructure.Repositories.Event.Interfaces  // IUnitOfWork defined here

// DTOs
Ticketing.Infrastructure.DTOs.Admin.Event.Request
Ticketing.Infrastructure.DTOs.Admin.Event.Response
Ticketing.Infrastructure.DTOs.Client.Event.Response

// Entities
Ticketing.Infrastructure.Entities.Event.Response
```

### Classes

| Type | Pattern | Example |
|------|---------|---------|
| Controller | `{Module}Controller` | `EventController`, `VenueController` |
| UseCase Interface | `I{Module}UseCases` | `IEventUseCases` |
| UseCase Class | `{Module}UseCases` | `EventUseCases` |
| Domain Interface | `I{Module}DomainService` | `IEventDomainService` |
| Domain Class | `{Module}DomainService` | `EventDomainService` |
| Repository Interface | `I{Module}Repository` | `IEventRepository` |
| Repository Class | `{Module}Repository` | `EventRepository` |
| UnitOfWork Interface | `I{Module}UnitOfWork` | `IEventUnitOfWork` |
| UnitOfWork Class | `{Module}UnitOfWork` | `EventUnitOfWork` |
| Entity | `{Module}Entity` | `EventEntity`, `VenueSeatEntity` |
| DTO Request | `{Action}{Module}Request` | `EventCreateRequest`, `EventUpdateRequest` |
| DTO Response | `{Module}{Context}Dto` | `EventDetailDto`, `EventClientListDto` |

### Properties & Variables

- **Local variables**: `camelCase`
  ```csharp
  var eventId = request.event_id;
  var pageSize = request.pagesize;
  ```

- **DTO/Entity properties**: `snake_case` (for PostgreSQL mapping)
  ```csharp
  public class EventEntity {
      public long event_id { get; set; }
      public string event_code { get; set; }
      public string event_name { get; set; }
  }
  ```

- **Private fields**: `_camelCase` or `camelCase`
  ```csharp
  private readonly IEventRepository _repository;
  private string eventCode;
  ```

- **Constants**: `UPPER_SNAKE_CASE`
  ```csharp
  private const string CACHE_KEY_PREFIX = "ticketing:";
  private const int HOLD_EXPIRY_MINUTES = 10;
  ```

### Methods

- **Public API methods**: `PascalCase`
  - `InsertAsync()`, `UpdateAsync()`, `GetByIdAsync()`, `GetPagedAsync()`
  - Action verbs: `Get`, `Insert`, `Update`, `Delete`, `Check`, `Export`

- **Private/Internal**: `PascalCase`
  ```csharp
  private async Task<bool> ValidateEventExists(long eventId);
  private void LogAuditTrail(string action, object data);
  ```

---

## Response & Error Handling

### Standard Response Wrapper

**All** API responses wrapped in `ResponseMessage<T>`:

```csharp
public class ResponseMessage<DataType> : IResponseMessage
{
    public bool issuccess { get; }
    public string status { get; set; }        // "success" | "error" | "warning"
    public string message { get; set; }        // Vietnamese message
    public int errorcode { get; set; }         // 0 on success, error code on failure
    public DataType data { get; set; }         // Returned data
    public IEnumerable<BaseError> errors { get; set; }  // Validation errors
}
```

### Success Response

```csharp
// HTTP 200 OK
{
  "issuccess": true,
  "status": "success",
  "message": "Thêm sự kiện thành công",
  "errorcode": 0,
  "data": { "event_id": 123 },
  "errors": []
}
```

### Error Response

```csharp
// HTTP 200 OK (status indicates error)
{
  "issuccess": false,
  "status": "error",
  "message": "Sự kiện không tìm thấy",
  "errorcode": 404,
  "data": null,
  "errors": []
}
```

### Validation Error Response

```csharp
// HTTP 400 Bad Request
{
  "issuccess": false,
  "status": "error",
  "message": "Validation failed",
  "errorcode": 400,
  "data": null,
  "errors": [
    {
      "property_message": "event_name",
      "error_message": "event_name là bắt buộc"
    }
  ]
}
```

### Factory Methods

```csharp
// Use fluent extensions for building responses
var response = new ResponseMessage<int>();
response.MessageSuccess(eventId, "Thêm sự kiện thành công");
return response;

// Or error
var response = ResponseMessage<EventDetailDto>.Error("Event not found");
```

### Message Localization

- **All messages in Vietnamese** (UTF-8 encoded)
- **Follow pattern**: `{Action} {Entity} {Result}`
  - ✅ "Thêm sự kiện thành công"
  - ✅ "Cập nhật venue thất bại"
  - ✅ "Xóa người dùng thành công"
  - ❌ "Success" or generic messages

---

## DTO Conventions

### Naming & Organization

```
DTOs/
├── Admin/
│   └── Event/
│       ├── EventDto.cs                    # Shared DTO
│       ├── Request/
│       │   ├── EventCreateRequest.cs
│       │   ├── EventUpdateRequest.cs
│       │   └── EventDeleteRequest.cs
│       └── Response/
│           ├── EventDetailResponse.cs
│           └── EventListResponse.cs
└── Client/
    └── Event/
        ├── Request/
        │   └── EventSearchClientRequest.cs
        └── Response/
            ├── EventClientListDto.cs
            └── EventClientDetailDto.cs
```

### Request DTO Structure

```csharp
public class EventCreateRequest
{
    [Required(ErrorMessage = "event_code là bắt buộc")]
    [StringLength(50)]
    public string event_code { get; set; } = string.Empty;

    [Required(ErrorMessage = "event_name là bắt buộc")]
    [StringLength(255)]
    public string event_name { get; set; } = string.Empty;

    [Range(1, long.MaxValue, ErrorMessage = "venue_id phải > 0")]
    public long venue_id { get; set; }

    public DateTime start_time { get; set; }
    public DateTime end_time { get; set; }
    public string status { get; set; } = "draft";

    // Helper method for Dapper (converts to parameter object)
    public object[] ToParameterArray()
    {
        // See Extension Methods section
    }
}
```

### Response DTO Structure

```csharp
public class EventDetailDto
{
    public long event_id { get; set; }
    public string event_code { get; set; }
    public string event_name { get; set; }
    public string status { get; set; }
    public DateTime created_at { get; set; }
    
    // Nested objects
    public List<EventZoneDto> zones { get; set; } = new();
}

public class EventZoneDto
{
    public long event_zone_id { get; set; }
    public string zone_name { get; set; }
    public decimal current_price { get; set; }
    
    public List<EventVenueSeatDto> seats { get; set; } = new();
}

public class EventVenueSeatDto
{
    public long seat_id { get; set; }
    public string seat_label { get; set; }
    public string status { get; set; }  // "available", "held", "sold"
}
```

### Property Mapping

- **snake_case** in DTOs matches PostgreSQL column names exactly
- Dapper auto-maps via reflection
- Use `[MapIgnore]` if field not from DB
- Always use default empty strings for string nullables at class level

---

## Extension Methods

### ToParameterArray()

Every request DTO has this method:

```csharp
public object[] ToParameterArray()
{
    return new object[]
    {
        nameof(event_code), event_code,
        nameof(event_name), event_name,
        nameof(description), description ?? "",
        nameof(venue_id), venue_id,
        nameof(banner_url), banner_url ?? "",
        nameof(start_time), start_time.ToDbTimestamp(),
        nameof(end_time), end_time.ToDbTimestamp(),
    };
}
```

**Convention**:
- Key is property name (unquoted)
- Value is property value
- DateTime converted via `ToDbTimestamp()` helper
- Null → empty string or default value
- Array alternating key-value pairs

### Custom Extension Methods

Located: `Ticketing.Infrastructure/Extensions/`

```csharp
// Convert DateTime to database timestamp (without timezone)
public static DateTime ToDbTimestamp(this DateTime value)
{
    return DateTime.SpecifyKind(value, DateTimeKind.Unspecified);
}

// Convert nullable DateTime
public static DateTime? ToDbTimestamp(this DateTime? value)
{
    return value.HasValue 
        ? DateTime.SpecifyKind(value.Value, DateTimeKind.Unspecified) 
        : null;
}

// Response extensions
public static ResponseMessage<T> MessageSuccess<T>(this ResponseMessage<T> response, T data, string message);
public static ResponseMessage<T> MessageError<T>(this ResponseMessage<T> response, string message);
```

---

## Validation Patterns

### Request Validation

Use **Data Annotations** in DTOs:

```csharp
[Required(ErrorMessage = "username là bắt buộc")]
[StringLength(50, ErrorMessage = "username không được vượt quá 50 ký tự")]
public string username { get; set; }

[EmailAddress(ErrorMessage = "email không hợp lệ")]
public string email { get; set; }

[Range(1, long.MaxValue, ErrorMessage = "venue_id phải lớn hơn 0")]
public long venue_id { get; set; }

[Range(typeof(DateTime), "01/01/2020", "01/01/2030")]
public DateTime start_time { get; set; }
```

### Business Logic Validation

In Domain Service:

```csharp
public async Task<ResponseMessage<int>> InsertAsync(EventEntity entity)
{
    // Validate business rules
    if (entity.end_time <= entity.start_time)
        return new ResponseMessage<int>()
            .MessageError("end_time phải sau start_time");
    
    if (entity.sale_end_time > entity.start_time)
        return new ResponseMessage<int>()
            .MessageError("sale_end_time phải trước start_time");
    
    // Proceed with insert
    ...
}
```

### Error Messages

**Format**: `{Field} {Condition} {Requirement}`
- Field name in Vietnamese or English (lowercase)
- Specific reason
- Actionable feedback

Examples:
- ✅ "event_code đã tồn tại trong hệ thống"
- ✅ "start_time phải trước sale_start_time"
- ✅ "quantity không được vượt quá số ghế còn lại"
- ❌ "Invalid input" (too generic)
- ❌ "Error 500" (not user-friendly)

---

## Transaction Management Pattern

Every write operation follows:

```csharp
public async Task<ResponseMessage<int>> InsertAsync(EventEntity entity, CancellationToken ct)
{
    await unitOfWork.OpenAsync(ct);
    
    try
    {
        await unitOfWork.BeginTransactionAsync(ct);
        
        // Multiple repository operations
        var id = await _repository.InsertAsync(entity, ct);
        
        if (string.IsNullOrWhiteSpace(id))
            return new ResponseMessage<int>()
                .MessageError("Thêm sự kiện thất bại");
        
        // More operations...
        
        await unitOfWork.CommitAsync(isCloseConnection: true, ct);
        
        return new ResponseMessage<int>()
            .MessageSuccess(int.Parse(id), "Thêm sự kiện thành công");
    }
    catch (Exception ex)
    {
        await unitOfWork.RollbackAsync(isCloseConnection: true, ct);
        return new ResponseMessage<int>()
            .MessageError($"Thêm sự kiện thất bại: {ex.Message}");
    }
}
```

**Key Points**:
- Open → Begin → Operations → Commit/Rollback → Close
- All operations in single transaction (consistency)
- Catch at domain level, log, return error response
- Never throw exceptions to controller (handled via ResponseMessage status)

---

## Dependency Injection Patterns

### Controller Injection

```csharp
public class EventController(
    IEventUseCases eventUseCases,
    IUserHelper userHelper) : BaseApiController
{
    [HttpPost("insert")]
    public async Task<IActionResult> Insert([FromBody] EventCreateRequest request, CancellationToken ct)
    {
        var result = await eventUseCases.InsertAsync(request, userHelper.UserId, ct);
        return Ok(result);
    }
}
```

### UseCase Injection

```csharp
public interface IEventUseCases
{
    Task<ResponseMessage<int>> InsertAsync(EventCreateRequest request, long userId, CancellationToken ct);
}

public class EventUseCases(IEventDomainService domainService) : IEventUseCases
{
    public async Task<ResponseMessage<int>> InsertAsync(EventCreateRequest request, long userId, CancellationToken ct)
    {
        var entity = MapRequestToEntity(request, userId);
        return await domainService.InsertAsync(entity, ct);
    }
}
```

### Domain Service Injection

```csharp
public class EventDomainService(
    IEventUnitOfWork unitOfWork,
    ICacheService cacheService) : IEventDomainService
{
    // UnitOfWork provides access to all Event-related repositories
    // CacheService provides Redis integration
}
```

### Registration (DI Files)

```csharp
// Ticketing.Infrastructure/Configurations/ConfigDI/InfrastructureConfigDI.cs
services.AddScoped<IEventRepository, EventRepository>();
services.AddScoped<IEventUnitOfWork, EventUnitOfWork>();

// Ticketing.Application/ConfigDI/UseCaseConfigureDI.cs
services.AddScoped<IEventUseCases, EventUseCases>();

// Ticketing.Domain/ConfigDI/DomainConfigDI.cs
services.AddScoped<IEventDomainService, EventDomainService>();
```

---

## Attribute Conventions

### Authorization

```csharp
[Authorize]  // Requires valid JWT
[AllowAnonymous]  // Skip auth (rare)
```

### HTTP Verbs

```csharp
[HttpGet("{id}")]           // Retrieve single resource
[HttpPost]                  // Create new resource
[HttpPut]                   // Update existing resource
[HttpDelete]                // Delete resource
[HttpPatch]                 // Partial update (rarely used)
```

### Route Patterns

```csharp
[Route("api/admin/event")]  // Admin endpoints
[Route("api/client/event")]  // Client/public endpoints
[Route("api/auth")]         // Auth endpoints
```

### Response Type Hints

```csharp
[ProducesResponseType(typeof(ResponseMessage<int>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
```

---

## Async/Await Conventions

- **Always** use `async/await` for I/O operations
- **Always** pass `CancellationToken` through the call stack
- **Never** block with `.Result` or `.Wait()`
- **Always** use `ConfigureAwait(false)` in libraries (optional here)

```csharp
public async Task<ResponseMessage<EventDetailDto>> GetByIdAsync(
    long eventId, 
    CancellationToken cancellationToken = default)
{
    var eventData = await _repository.GetAsync<EventDetailDto>(
        new { event_id = eventId }, 
        cancellationToken);
    
    if (eventData == null)
        return new ResponseMessage<EventDetailDto>()
            .MessageError("Sự kiện không tìm thấy");
    
    return new ResponseMessage<EventDetailDto>()
        .MessageSuccess(eventData, "Lấy chi tiết sự kiện thành công");
}
```

---

## Logging Conventions

Using **Serilog**:

```csharp
private readonly ILogger<EventDomainService> _logger;

public EventDomainService(ILogger<EventDomainService> logger)
{
    _logger = logger;
}

// Log information
_logger.LogInformation("Event {EventId} created by user {UserId}", eventId, userId);

// Log errors
_logger.LogError(ex, "Failed to create event {EventId}", eventId);

// Log warnings
_logger.LogWarning("Event {EventId} status changed from {OldStatus} to {NewStatus}", 
    eventId, oldStatus, newStatus);
```

**Structured Logging**: Property names in curly braces for Serilog parsing.

---

## Type Safety & Nullability

Project configured with:
- `<Nullable>enable</Nullable>` — Strict null checking
- `<ImplicitUsings>enable</ImplicitUsings>` — Global using statements

**Conventions**:
- Use `string?` for nullable strings
- Use `T?` for nullable value types
- Use `null!` only when you're absolutely certain
- Validate null before use

```csharp
public class EventEntity
{
    public string event_code { get; set; } = string.Empty;  // Never null
    
    public string? description { get; set; }  // Can be null
    
    public DateTime start_time { get; set; }  // Never null
    
    public DateTime? published_at { get; set; }  // Can be null
}
```


