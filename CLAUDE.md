# Ticketing System - Project Overview for AI Assistants

## Quick Reference

This is an **Online Event Ticketing System** built with .NET 8, PostgreSQL, and Redis. It handles high-concurrency seat reservations with real-time inventory tracking.

---

## Tech Stack & Versions

| Component | Version | Notes |
|-----------|---------|-------|
| **.NET SDK** | 8.0.0 | Nullable enabled, Implicit usings enabled |
| **ASP.NET Core** | 8.0.x | Web API |
| **PostgreSQL** | 14+ (in Docker) | Source of truth for all data |
| **Redis** | 6.0+ | Session cache, distributed locks, seat inventory cache |
| **Npgsql** | (via .csproj) | PostgreSQL ADO.NET provider |
| **Dapper** | Implicit | Used for stored procedure execution via custom helpers |
| **Serilog** | 10.0.0 | Structured logging to console |
| **JWT** | Built-in (JwtBearer 8.0.25) | Token-based authentication |
| **Swagger/Swashbuckle** | 6.6.2 | OpenAPI documentation |

---

## Solution Structure

```
TicketingSystem/                          # Root solution
├── TicketingSystem.sln                  # Solution file (4 projects)
├── global.json                          # SDK version lock (8.0.0)
├── appsettings.json                     # Configuration
│
├── TicketingSystem/                     # Main Web API project
│   ├── Program.cs                       # Startup configuration
│   ├── Controllers/
│   │   ├── Admin/                       # Admin endpoints
│   │   └── Client/                      # Client/public endpoints
│   ├── BackgroundServices/              # IHostedService implementations
│   │   └── SeatHoldExpiryBackgroundService.cs  # Releases expired holds every 60s
│   ├── Middleware/                      # ExceptionHandlingMiddleware
│   ├── Configures/                      # Reserved for future configs
│   └── docs/                            # API documentation
│
├── Ticketing.Application/               # Application layer (Use Cases)
│   ├── UseCases/
│   │   ├── Event/
│   │   ├── Venue/
│   │   ├── SysRole/
│   │   ├── Admin/
│   │   └── Client/
│   └── ConfigDI/                        # Dependency injection registration
│
├── Ticketing.Domain/                    # Domain layer (Business logic)
│   ├── Domain/                          # Domain services (orchestration)
│   ├── Constants/                       # Business constants
│   └── ConfigDI/                        # DI registration
│
├── Ticketing.Infrastructure/            # Infrastructure layer
│   ├── Repositories/                    # Data access layer
│   │   ├── Event/
│   │   ├── Venue/
│   │   ├── SeatHold/
│   │   ├── Payment/
│   │   └── (others)
│   ├── Entities/                        # Entity models for Dapper
│   ├── DTOs/                            # Request/Response DTOs
│   ├── Configurations/
│   │   ├── Impl/                        # DapperContext, Accessor
│   │   └── ConfigDI/                    # DI registration
│   ├── Helpers/                         # Utilities (JWT, Cache, etc.)
│   ├── Constants/                       # Infrastructure constants
│   ├── Persistence/                     # Dapper procedure helpers
│   └── Extensions/                      # Extension methods
│
└── database/                            # Database scripts
    └── functions/                       # PostgreSQL stored procedures
```

---

## Architecture at a Glance

**Pattern**: Clean Architecture with Dependency Injection

```
Request → Controller
   ↓
UseCase Interface (Application layer)
   ↓
Domain Service (Business logic)
   ↓
Repository (Data access via Dapper + Stored Procedures)
   ↓
PostgreSQL Database
```

**Data Flow for Insert/Update/Delete**:
1. Controller receives request, extracts `user_id` from JWT
2. Calls UseCase method with DTO + `user_id` + `CancellationToken`
3. UseCase maps DTO to Entity
4. Domain Service opens transaction via UnitOfWork
5. Domain Service calls Repository methods
6. Repository calls Dapper helper → PostgreSQL function → refcursor
7. Returns ID on success, null on failure
8. Domain Service commits/rollbacks transaction
9. Uses ResponseMessage<T> wrapper for response

**Data Flow for Read (GetById/GetPaged)**:
1. No transaction needed (read-only)
2. Repository directly calls Dapper
3. Results mapped to DTO
4. Returned wrapped in ResponseMessage<T>

---

## Core Dependencies & Responsibilities

### Layers

| Layer | Location | Responsibility |
|-------|----------|-----------------|
| **Presentation** | `TicketingSystem/Controllers/` | HTTP endpoints, route definitions, model validation |
| **Application** | `Ticketing.Application/UseCases/` | Use case orchestration, DTO mapping, response building |
| **Domain** | `Ticketing.Domain/Domain/` | Business rule enforcement, transaction orchestration |
| **Infrastructure** | `Ticketing.Infrastructure/` | DB access, Dapper integration, caching, JWT |

### Key Services

- **DapperContext**: Creates PostgreSQL connections
- **DapperContextAccessor**: Scoped context holder (connection + transaction)
- **DapperProcedureHelper**: Executes stored procedures, handles refcursor
- **Repository<TEntity>**: Generic CRUD operations via stored procedures
- **UnitOfWork**: Transaction management (`CommitAsync`/`RollbackAsync` both call `CloseAsync`, clearing `DapperContextAccessor`)
- **ICacheService**: Redis integration
- **IUserHelper**: Extracts user info from JWT claims
- **SeatHoldExpiryBackgroundService**: Singleton `IHostedService`, runs every 60s, creates a DI scope per run to resolve scoped `IBookingUseCases` and release expired holds

### Booking Module Notes

- **Idempotency key**: Client-generated UUID, scoped server-side as `hold-evt{event_id}_cust{customerId}_{uuid}` to prevent cross-customer collision
- **Idempotency record states**: `processing` → `completed` (with `response_snapshot`) or `failed`; retry allowed for `failed`, expired `processing`, or broken `completed`
- **`response_snapshot` field**: Always use `string.Empty` (never `null`) — Npgsql cannot infer PostgreSQL type from `DBNull.Value`
- **Checkout payment**: Mock only — no real payment gateway; order is created in `paid` status immediately
- **Background release isolation**: Each expired hold is released in its own transaction via `ProcessExpiredRelease`; failure of one hold does not block others because `RollbackAsync` closes and clears the connection

---

## Important Rules & Conventions

### Response Format

All responses wrapped in **ResponseMessage<T>**:

```csharp
{
  "issuccess": true,
  "status": "success" | "error" | "warning",
  "message": "Vi message",
  "errorcode": 0 (on success) or non-zero on error,
  "data": { /* T */ },
  "errors": [ /* validation errors */ ]
}
```

### Architecture Patterns

1. **Stored Procedures Only**: No raw SQL. All data access via PostgreSQL functions.
2. **Refcursor Pattern**: Functions return refcursor. Dapper fetches results via `FETCH ALL IN "cursor_name"`.
3. **Soft Delete**: All entities have `is_deleted` flag. Never physically delete.
4. **Audit Trail**: Insert/Update include `created_by`, `updated_by`, timestamps.
5. **snake_case DTOs**: Database columns → DTO properties use snake_case for direct mapping.

### Naming Conventions

- **Controllers**: `{Module}Controller` (e.g., `EventController`)
- **Use Cases**: `I{Module}UseCases` interface, `{Module}UseCases` class
- **Domain Services**: `I{Module}DomainService` interface, `{Module}DomainService` class
- **Repositories**: `I{Module}Repository` interface, `{Module}Repository` class
- **Entities**: `{Module}Entity`
- **Routes**: `/api/admin/{resource}` for admin, `/api/client/{resource}` for public

### Dependency Injection

DI registration files:
- `Ticketing.Infrastructure/Configurations/ConfigDI/InfrastructureConfigDI.cs`
- `Ticketing.Application/ConfigDI/UseCaseConfigureDI.cs`
- `Ticketing.Domain/ConfigDI/DomainConfigDI.cs`

All registered as **Scoped** (per HTTP request) except:
- **DapperContext** — Singleton
- **SeatHoldExpiryBackgroundService** — Singleton (`AddHostedService`); uses `IServiceScopeFactory` to resolve scoped dependencies per run

### Authentication

- **JWT Bearer**: Token in `Authorization: Bearer <token>` header
- **Claims**: `NameIdentifier` (user_id), `Name` (username), claims for roles
- **[Authorize]** attribute on all admin endpoints
- Public endpoints (Client) generally don't require auth
- Roles extracted from JWT claims under `role` key

### Error Handling

- **Middleware**: `ExceptionHandlingMiddleware` catches unhandled exceptions
- **Returns**: `ResponseMessage<T>` with status `error`
- **HTTP Status**: 500 for unhandled exceptions
- **Validation**: ASP.NET model validation → `ResponseMessage<T>` with `errors[]`

### Transactions

Managed via **UnitOfWork** pattern:
```csharp
await unitOfWork.OpenAsync(cancellationToken);
await unitOfWork.BeginTransactionAsync(cancellationToken);
// ... repository operations
await unitOfWork.CommitAsync(cancellationToken);  // or RollbackAsync
```

Transaction lives in `DapperContextAccessor` (scoped), shared across all repositories in same request.

### Database

- **Connection String**: From `appsettings.json` → `ConnectionStrings:DefaultConnection`
- **Schema**: All tables in `ticketing` schema
- **Functions**: Format `{schema}.{table}_{action}` (e.g., `ticketing.event_insert`)

---

## Related Documentation

- **`docs/ARCHITECTURE.md`** - Detailed request flows, module interactions, real-time features
- **`docs/DATABASE.md`** - Entity schemas, relationships, indexes, migrations
- **`docs/CONVENTIONS.md`** - Detailed naming rules, response formats, validation patterns
- **`docs/API.md`** - All endpoints, auth requirements, request/response examples

---

## Quick Start Commands

```bash
# Build
dotnet build

# Run (requires PostgreSQL + Redis in Docker)
docker-compose up -d
dotnet run --project TicketingSystem/TicketingSystem.csproj

# API available at: http://localhost:5000 (or configured port)
# Swagger: http://localhost:5000/swagger
```

---

## Common Tasks

### Adding a New Endpoint

1. Create Entity in `Ticketing.Infrastructure/Entities/{Module}/`
2. Create DTOs in `Ticketing.Infrastructure/DTOs/{Module}/Request|Response/`
3. Create PostgreSQL function (`database/functions/`)
4. Create Repository in `Ticketing.Infrastructure/Repositories/{Module}/`
5. Create Domain Service in `Ticketing.Domain/Domain/{Module}/`
6. Create UseCase in `Ticketing.Application/UseCases/{Module}/`
7. Create Controller method in `TicketingSystem/Controllers/{Area}/{Module}/`
8. Register in 3 DI files (Infrastructure, Domain, Application)

See `CRUD_CODEGEN_GUIDE.md` for detailed checklist.

### Debugging

- Serilog logs to console by default
- Database connection validated on startup
- Redis connection optional but recommended for production
- Use `ILogger<T>` for logging (injected automatically)

---

## Production Considerations

- Redis strongly recommended for seat-level locking (prevent overselling)
- PostgreSQL connection pooling via Npgsql
- JWT secret key must be 16+ bytes (HS256 requirement)
- CORS configured for localhost:3000 by default (update for production)
- Soft deletes ensure audit trail
- Stored procedures provide SQL-level consistency


