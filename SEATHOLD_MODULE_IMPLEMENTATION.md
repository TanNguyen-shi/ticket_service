# SeatHold Module - Implementation Summary

## Overview
Successfully created Entity, DTO, and Repository layers for SeatHold and SeatHoldItem modules following the established Clean Architecture pattern.

## Files Created

### 1. Entity Models

#### SeatHoldEntity.cs
- **Path**: `Ticketing.Infrastructure/Entities/SeatHold/Response/SeatHoldEntity.cs`
- **Inherits**: `BaseEntity`
- **Properties**:
  - `hold_id`: bigint (PK)
  - `hold_code`: string
  - `event_id`: bigint (FK)
  - `user_id`: bigint (FK)
  - `status`: string (active, expired, released, converted, cancelled)
  - `hold_started_at`: DateTime
  - `hold_expires_at`: DateTime
  - `released_at`: DateTime? (nullable)
  - `release_reason`: string? (nullable)
  - Plus inherited BaseEntity fields: created_at, updated_at, created_by, updated_by, deleted_by, is_deleted

#### SeatHoldItemEntity.cs
- **Path**: `Ticketing.Infrastructure/Entities/SeatHold/Response/SeatHoldItemEntity.cs`
- **Inherits**: `BaseEntity`
- **Properties**:
  - `hold_item_id`: bigint (PK)
  - `hold_id`: bigint (FK)
  - `event_seat_inventory_id`: bigint
  - `seat_id`: bigint
  - `zone_id`: bigint
  - `price_at_hold`: decimal
  - `seat_label_snapshot`: string? (snapshot of seat label at hold time)
  - `zone_name_snapshot`: string? (snapshot of zone name at hold time)
  - `status`: string (active, released, converted, expired)
  - Plus inherited BaseEntity fields

### 2. DTO Models

#### SeatHold Request DTOs
- **Path**: `Ticketing.Infrastructure/DTOs/Client/SeatHold/Request/SeatHoldRequestDto.cs`
- **DTOs**:
  - `SeatHoldInsertDto`: For creating new seat holds
  - `SeatHoldUpdateDto`: For updating existing seat holds
  - `SeatHoldCheckDto`: For checking duplicate hold codes
  - `SeatHoldDeleteDto`: For deleting seat holds
  - `SeatHoldGetByIdDto`: For retrieving single seat hold
  - `SeatHoldGetPagedListDto`: For paginated list queries

#### SeatHold Response DTOs
- **Path**: `Ticketing.Infrastructure/DTOs/Client/SeatHold/Response/SeatHoldResponseDto.cs`
- **DTOs**:
  - `SeatHoldResponseDto`: Single seat hold with related event and user details
  - `SeatHoldPagedDto`: Paginated result with row_index and row_total

#### SeatHoldItem Request DTOs
- **Path**: `Ticketing.Infrastructure/DTOs/Client/SeatHold/Request/SeatHoldItemRequestDto.cs`
- **DTOs**:
  - `SeatHoldItemInsertDto`: For creating new hold items
  - `SeatHoldItemUpdateDto`: For updating hold items
  - `SeatHoldItemCheckDto`: For checking duplicate items
  - `SeatHoldItemDeleteDto`: For deleting hold items
  - `SeatHoldItemGetByIdDto`: For retrieving single item
  - `SeatHoldItemGetPagedListDto`: For paginated list queries

#### SeatHoldItem Response DTOs
- **Path**: `Ticketing.Infrastructure/DTOs/Client/SeatHold/Response/SeatHoldItemResponseDto.cs`
- **DTOs**:
  - `SeatHoldItemResponseDto`: Single item with seat, zone, and event details
  - `SeatHoldItemPagedDto`: Paginated result with related data

### 3. Repository Classes

#### SeatHoldRepository.cs
- **Path**: `Ticketing.Infrastructure/Repositories/SeatHold/SeatHoldRepository.cs`
- **Interface**: `ISeatHoldRepository : IGenericRepository<SeatHoldEntity>`
- **Methods**:
  - Inherited from `IGenericRepository<T>`:
    - `InsertAsync<TParam>()`: Call `ticketing.seat_hold_insert`
    - `UpdateAsync<TParam>()`: Call `ticketing.seat_hold_update`
    - `CheckExistAsync<TParam>()`: Call `ticketing.seat_hold_check`
    - `DeleteAsync<TParam>()`: Call `ticketing.seat_hold_delete`
    - `GetAsync<TResult, TParam>()`: Call `ticketing.seat_hold_getbyid`
    - `GetPagedAsync<TResult, TParam>()`: Call `ticketing.seat_hold_getpagedlist`
  - Custom methods:
    - `GetByEventIdAsync<TResult>()`: Call `ticketing.seat_hold_getbyeventid`
    - `GetByUserIdAsync<TResult>()`: Call `ticketing.seat_hold_getbyuserid`

#### SeatHoldItemRepository.cs
- **Path**: `Ticketing.Infrastructure/Repositories/SeatHold/SeatHoldItemRepository.cs`
- **Interface**: `ISeatHoldItemRepository : IGenericRepository<SeatHoldItemEntity>`
- **Methods**:
  - Inherited from `IGenericRepository<T>` (same as SeatHoldRepository)
  - Custom method:
    - `GetByHoldIdAsync<TResult>()`: Call `ticketing.seat_hold_item_getbyholdid`

### 4. Unit of Work

#### SeatHoldUnitOfWork.cs
- **Path**: `Ticketing.Infrastructure/Repositories/SeatHold/SeatHoldUnitOfWork.cs`
- **Interface**: `ISeatHoldUnitOfWork : IUnitOfWork`
- **Properties**:
  - `SeatHoldRepository`: ISeatHoldRepository
  - `SeatHoldItemRepository`: ISeatHoldItemRepository
- **Purpose**: Manages transaction scope for both SeatHold and SeatHoldItem entities

### 5. SQL Functions

#### seat_hold_functions.sql
- **Path**: `database/functions/seat_hold_functions.sql`
- **SeatHold Functions**:
  - `seat_hold_check()`: Check for duplicate hold codes
  - `seat_hold_insert()`: Insert new seat hold
  - `seat_hold_update()`: Update existing seat hold
  - `seat_hold_delete()`: Delete seat hold
  - `seat_hold_getbyid()`: Get single hold with related data
  - `seat_hold_getpagedlist()`: Get paginated list with filters
  - `seat_hold_getbyeventid()`: Get all holds for an event
  - `seat_hold_getbyuserid()`: Get all holds for a user

- **SeatHoldItem Functions**:
  - `seat_hold_item_check()`: Check for duplicate items
  - `seat_hold_item_insert()`: Insert new hold item
  - `seat_hold_item_update()`: Update hold item
  - `seat_hold_item_delete()`: Delete hold item
  - `seat_hold_item_getbyid()`: Get single item with all related data
  - `seat_hold_item_getpagedlist()`: Get paginated list with filters
  - `seat_hold_item_getbyholdid()`: Get all items for a specific hold

## Architecture Pattern

### Folder Structure
```
Ticketing.Infrastructure/
├── Entities/
│   └── SeatHold/
│       └── Response/
│           ├── SeatHoldEntity.cs
│           └── SeatHoldItemEntity.cs
├── DTOs/
│   └── Client/
│       └── SeatHold/
│           ├── Request/
│           │   ├── SeatHoldRequestDto.cs
│           │   └── SeatHoldItemRequestDto.cs
│           └── Response/
│               ├── SeatHoldResponseDto.cs
│               └── SeatHoldItemResponseDto.cs
└── Repositories/
    └── SeatHold/
        ├── SeatHoldRepository.cs
        ├── SeatHoldItemRepository.cs
        └── SeatHoldUnitOfWork.cs

database/
└── functions/
    └── seat_hold_functions.sql
```

## Key Features

### 1. Generic Repository Pattern
- All repositories extend `Repository<TEntity>` which provides standard CRUD operations
- Custom methods for specific queries (GetByEventId, GetByUserId, GetByHoldId)
- Automatic stored procedure naming via `GetSpName()` helper

### 2. Transaction Management
- `SeatHoldUnitOfWork` provides shared transaction scope
- Both repositories can participate in the same transaction
- Connection and transaction objects managed by `DapperContextAccessor`

### 3. Data Snapshot Pattern
- SeatHoldItem captures seat and zone information at hold time via snapshot fields
- `seat_label_snapshot` and `zone_name_snapshot` preserve data for historical reference

### 4. Pagination Support
- All `GetPagedList` functions support:
  - Paginated results with `row_index` and `row_total`
  - Multiple filter criteria
  - Search keywords
  - Status filtering

## Next Steps

### 1. Register Repositories in DI Configuration
Add to `InfrastructureConfigDI.cs`:
```csharp
services.AddScoped<ISeatHoldUnitOfWork>(provider =>
{
    var dapperContext = provider.GetRequiredService<DapperContext>();
    var contextAccessor = provider.GetRequiredService<DapperContextAccessor>();
    var dapperHelper = provider.GetRequiredService<IDapperProcedureHelper>();
    
    return new SeatHoldUnitOfWork(dapperContext, contextAccessor, dapperHelper);
});
```

### 2. Create Domain Services (Optional)
- `SeatHoldDomainService`: Business logic for seat hold operations
- `SeatHoldValidationService`: Validation rules

### 3. Create Use Cases (Application Layer)
- `HoldSeatUseCase`: Create seat hold
- `ReleaseHoldUseCase`: Release seat hold
- `GetUserHoldsUseCase`: Get all holds for user

### 4. Create Controllers (Presentation Layer)
- `BookingController`: Endpoints for seat hold operations
- Routes: `api/client/booking/hold-seat`, `api/client/booking/release-hold`, etc.

### 5. Execute SQL Functions
Run `database/functions/seat_hold_functions.sql` against PostgreSQL database to create all stored procedures.

## Naming Conventions

- **Properties**: snake_case (e.g., `hold_id`, `event_id`, `seat_label_snapshot`)
- **Stored Procedures**: `schema_table_action` (e.g., `ticketing.seat_hold_getbyeventid`)
- **Repository Methods**: PascalCase Async (e.g., `GetByEventIdAsync<TResult>()`)
- **DTOs**: Suffixed with Dto (e.g., `SeatHoldInsertDto`, `SeatHoldResponseDto`)

## Dependencies

- **Framework**: .NET 8.0+ (C#)
- **Database**: PostgreSQL
- **ORM**: Dapper with refcursor stored procedures
- **Dependencies**:
  - `Npgsql`: PostgreSQL connection management
  - `Dapper`: Lightweight ORM for stored procedure execution
  - `IDapperProcedureHelper`: Custom helper for Dapper operations

## Testing Endpoints

Once integrated, you can test using:

```bash
# Create a seat hold
POST /api/client/booking/hold-seats
{
  "event_id": 5,
  "seat_ids": [16101, 16102]
}

# Get user's holds
GET /api/client/booking/my-holds

# Release a hold
POST /api/client/booking/release-hold/1

# Get hold details
GET /api/client/booking/holds/1
```

## Performance Notes

- All queries use prepared statements via stored procedures
- Batch queries supported for multiple event_ids or user_ids
- Pagination prevents loading large result sets
- Index recommendations for seat_hold: (event_id, user_id), (status, hold_expires_at)

