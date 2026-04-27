# System Architecture

## What This System Does

**Ticketing System** is an online event ticketing platform that:
- Allows admins to create and manage events, venues, seating zones
- Manages seat inventory and pricing tiers per zone
- Enables clients to browse events and hold seats (10-minute reserve)
- Tracks orders, payments, and ticket issuance
- Provides real-time seat status updates via SignalR (planned)
- Prevents overselling through distributed locking

**Target**: High-concurrency, consistent under heavy load (e.g., popular concert on sale).

---

## Request Flow - High Level

```
┌─────────────────────────────────────────────────────────────────┐
│ 1. Client/Admin sends HTTP request (with JWT token)             │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│ 2. BaseApiController intercepts                                  │
│    - [Authorize] checks JWT validity                            │
│    - IUserHelper extracts user_id from claims                   │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│ 3. Concrete Controller (e.g., EventController)                  │
│    - Receives DTO from [FromBody] or [FromQuery]               │
│    - Calls I{Module}UseCases method(user.UserId, dto, ...)     │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│ 4. UseCase Layer (Ticketing.Application)                        │
│    - Maps DTO → Entity                                          │
│    - Calls I{Module}DomainService method                        │
│    - Wraps result in ResponseMessage<T>                         │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│ 5. Domain Service (Ticketing.Domain)                            │
│    For Write Operations (Insert/Update/Delete):                │
│    ├─ Opens connection via IUnitOfWork                          │
│    ├─ Begins transaction                                        │
│    ├─ Calls Repository methods                                  │
│    ├─ Commits on success                                        │
│    └─ Rolls back on error                                       │
│                                                                 │
│    For Read Operations (Get/List):                             │
│    ├─ No transaction needed                                     │
│    └─ Directly queries repository                               │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│ 6. Repository (Ticketing.Infrastructure)                        │
│    - Inherits from Repository<TEntity>                          │
│    - Calls IDapperProcedureHelper                               │
│    - Passes object param with snake_case properties             │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│ 7. Dapper Procedure Helper                                      │
│    - Builds function call: schema.table_action(...)            │
│    - Executes via Npgsql                                        │
│    - Extracts cursor name from first result column              │
│    - Fetches results via FETCH ALL IN "cursor_name"            │
│    - Maps to TResult                                            │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│ 8. PostgreSQL                                                   │
│    - Executes stored function                                   │
│    - Returns refcursor with results                             │
│    - Enforces transactions at DB level                          │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│ 9. Response wrapped in ResponseMessage<T>                       │
│    - HTTP 200 OK                                                │
│    - Contains status, message, data, errors                     │
└─────────────────────────────────────────────────────────────────┘
```

---

## Module Architecture

### Core Modules

#### 1. **Event Management** (Event, EventZone, EventZonePrice)
- **Admin**: Create/Update/Delete events, define zones, set pricing
- **Client**: Browse featured/trending/upcoming events
- **Entities**: Event, EventZone, EventZonePrice
- **Domain Service**: EventDomainService
- **Use Cases**: IEventUseCases (Admin), IEventClientUseCases (Client)
- **Caching**: 5min TTL for event details, 3min for client lists

#### 2. **Venue Management** (Venue, VenueSection, VenueSeat)
- **Admin**: Define venues, sections (zones within venue), seats
- **Entities**: Venue, VenueSection, VenueSeat
- **Domain Service**: VenueDomainService
- **Use Cases**: IVenueUseCases

#### 3. **Seat Hold & Inventory**
- **Seat Inventory**: Real-time seat status (available/held/sold)
- **Seat Hold**: 10-minute reserve per user per event
- **Idempotency**: Prevent double-hold via idempotency key + request hash
- **Entities**: EventSeatInventory, SeatHold, SeatHoldItem, IdempotencyRequest
- **Domain Service**: Orchestrates hold flow, coordinates with Payment module
- **Use Cases**: IBookingUseCases (client-side holds)
- **Locking**: Redis distributed locks per seat (prevent race conditions)

#### 4. **Order & Ticket**
- **Ticket Order**: Group of seats + total price
- **Ticket Order Item**: Individual seat within order
- **Ticket**: Issued after payment confirmed
- **Entities**: TicketOrder, TicketOrderItem, Ticket
- **Workflow**: Hold → Checkout → Payment → Ticket Issue

#### 5. **Payment**
- **Payment Transaction**: Integrate with payment gateways (VNPay, Momo, Mock)
- **Payment Callback Log**: Audit trail of payment confirmations
- **Idempotency**: Prevent duplicate charges
- **Entities**: PaymentTransaction, PaymentCallbackLog
- **Domain Service**: Handles payment processing, callback validation

#### 6. **System Admin** (SysRole, SysUser, SysUserRole)
- **User Management**: Create/Update/Delete users
- **Role Management**: Define roles (ADMIN, STAFF, USER)
- **User-Role Mapping**: Assign roles to users
- **Entities**: SysUser, SysRole, SysUserRole

#### 7. **Audit & Logging**
- **Event Publish Log**: Track event status changes (draft → published → on_sale → …)
- **Audit Log**: Track all data changes (who, when, what)

---

## Database Schema - Key Entities

### Event & Ticketing
```
Event (1) ──────→ (M) EventZone
Event (1) ──────→ (M) EventZonePrice
Event (1) ──────→ (M) EventZoneSection
EventZone (1) ───────→ (M) EventZonePrice
EventZoneSection (1) ───→ (M) VenueSection
```

### Venue & Seats
```
Venue (1) ──────→ (M) VenueSection
VenueSection (1) ──────→ (M) VenueSeat
EventZoneSection (1) ──────→ (1) VenueSection
```

### Inventory & Orders
```
Event (1) ──────→ (M) EventSeatInventory
EventSeatInventory (1) ──────→ (M) TicketOrderItem  
EventSeatInventory (1) ──────→ (M) SeatHoldItem
SeatHold (1) ──────→ (M) SeatHoldItem
TicketOrder (1) ──────→ (M) TicketOrderItem
TicketOrderItem (1) ──────→ (1) Ticket
```

### Payments
```
TicketOrder (1) ──────→ (M) PaymentTransaction
PaymentTransaction (1) ──────→ (M) PaymentCallbackLog
```

### Idempotency & Audit
```
IdempotencyRequest - Prevents duplicate operations
PaymentCallbackLog - Logs all payment callbacks
EventPublishLog - Tracks event status changes
AuditLog - Logs all entity changes (user, module, action, old/new data)
```

---

## Request Flow for Key Operations

### 1. Admin Creates Event

```
EventController.Insert(EventCreateRequest)
  ↓
EventUseCases.InsertAsync(request, userId, cancellationToken)
  ├─ Map request → EventEntity
  ├─ Validate request fields
  └─ Call EventDomainService.InsertAsync(entity)
    ├─ Open connection
    ├─ Begin transaction
    ├─ Call EventRepository.InsertAsync(event_code, event_name, ...)
    │  └─ Execute ticketing.event_insert() → returns event_id
    ├─ Return ResponseMessage with event_id
    ├─ Commit transaction
    └─ Invalidate client event cache (version key)
  └─ Return to controller
    └─ HTTP 200 OK with ResponseMessage<int>
```

### 2. Client Holds Seats (Complex Flow)

```
BookingController.HoldSeats(HoldSeatsRequest)
  ├─ Extract user_id from JWT
  └─ Call BookingUseCases.HoldSeatsAsync(request, userId, cancellationToken)
    ├─ Validate request (seat_ids not empty, event exists)
    ├─ Calculate request_hash (userId + eventId + sorted_seat_ids)
    ├─ Check IdempotencyRequest (prevent double-click)
    │  ├─ If exists + status=completed → return cached response
    │  ├─ If exists + status=processing → return "already processing" error
    │  ├─ If exists + status=failed → allow retry (new request)
    │  └─ If not exists → create new record with status=processing
    ├─ Acquire Redis locks for each seat (lock:event:{eventId}:seat:{seatId})
    │  └─ If any lock fails → release all, return error
    ├─ Call EventDomainService.HoldSeatsAsync(...)
    │  ├─ Open connection
    │  ├─ Begin transaction
    │  ├─ For each seat:
    │  │  ├─ Insert SeatHoldItem (hold_id, seat_id, price_at_hold, ...)
    │  │  ├─ Update EventSeatInventory (optimistic lock: status=available → held)
    │  ├─ Commit transaction
    │  ├─ Update IdempotencyRequest (status=completed, response_snapshot=hold_id)
    │  ├─ Schedule auto-expire job (10 minutes)
    │  └─ Broadcast SeatHeld via SignalR (event group)
    └─ Return HoldSeatsResponse with hold_id, hold_code, expires_at
      └─ HTTP 200 OK
```

### 3. Client Checkout (After Hold)

```
BookingController.Checkout(CheckoutRequest)
  ├─ Validate hold still valid
  ├─ Check hold hasn't expired
  ├─ Convert SeatHold to TicketOrder
  │  ├─ Create TicketOrder (hold_id → order_id, calculate total_price)
  │  ├─ For each SeatHoldItem:
  │  │  └─ Create TicketOrderItem
  │  ├─ Mark SeatHold as converted
  │  └─ Update EventSeatInventory (held → sold)
  └─ Initiate payment flow
    ├─ Create PaymentTransaction (status=pending)
    └─ Return payment gateway URL
```

### 4. Payment Callback (From Provider)

```
PaymentCallbackController.Callback(CallbackPayload)
  ├─ Verify callback signature
  ├─ Log in PaymentCallbackLog (received, not yet processed)
  ├─ Check IdempotencyRequest (prevent duplicate processing)
  ├─ Update PaymentTransaction (success/failed)
  ├─ If success:
  │  ├─ Update TicketOrder (status=confirmed)
  │  ├─ Create Ticket records for each order item
  │  ├─ Update EventSeatInventory (complete transition)
  │  └─ Send confirmation email
  ├─ Update IdempotencyRequest (status=completed)
  └─ Return 200 OK (webhook acknowledgment)
```

### 5. Seat Hold Auto-Expire (Scheduled Job)

```
SeatHoldExpirationJob.Execute(hold_id)
  ├─ Check if SeatHold.hold_expires_at <= now()
  ├─ Mark SeatHold as expired
  ├─ For each SeatHoldItem:
  │  ├─ Mark as expired
  │  ├─ Update EventSeatInventory (held → available, version++)
  │  └─ Release Redis lock
  └─ Broadcast SeatReleased via SignalR
```

---

## Caching Strategy

| Key | TTL | Purpose | Invalidation |
|-----|-----|---------|--------------|
| `event:detail:{eventId}` | 5 min | Event detail + zones + prices | On event update |
| `event:client:featured` | 3 min | Featured events list | Version key |
| `event:client:trending` | 3 min | Trending events list | Version key |
| `event:client:upcoming` | 3 min | Upcoming events list | Version key |
| `venue:detail:{venueId}` | 30 min | Venue + sections + seats | On venue update |
| `ticketing:event:client:version` | 30 days | Cache version for all client event caches | Manual invalidate on any event change |
| `lock:event:{eventId}:seat:{seatId}` | 20 sec | Distributed seat lock | Manual release or timeout |

---

## Transaction Boundaries

**Write Operations Always Transactional**:
- EventDomainService.InsertAsync/UpdateAsync/DeleteAsync
- Domain services always use UnitOfWork
- Rollback on any error

**Read Operations Non-transactional**:
- Repositories directly query
- No transaction overhead for reads

**Transactions Storage**: 
- Scoped `DapperContextAccessor` (per HTTP request)
- All repositories share same connection + transaction within request scope
- Multiple operations within single transaction if all via same UnitOfWork instance

---

## Real-Time Features (Planned)

### SignalR Integration
- **Group**: `event-details-{eventId}` for each event
- **Events Broadcast**:
  - `SeatHeld`: User holds seats
  - `SeatReleased`: Hold expires or user releases
  - `SeatSold`: Checkout completed
  - `EventStatusChanged`: Admin changes event status
  
### Client Notification
- Browser connects to SignalR hub (authenticated)
- Receives real-time seat inventory updates
- Updates UI without page refresh

---

## Error Handling Strategy

### Middleware Level
```csharp
ExceptionHandlingMiddleware catches all unhandled exceptions
  → Logs via Serilog
  → Returns HTTP 500 + ResponseMessage<T> with status=error
```

### Domain Level
```csharp
Domain services validate business rules
  → Return Failed ResponseMessage<T> (HTTP 200 but status=error)
  → Domain validation errors don't throw exceptions
```

### Repository Level
```csharp
Database operations wrapped in try-catch
  → Rollback transaction on error
  → Pass error up to domain service
```

### Idempotency
```csharp
Duplicate requests detected via idempotency_key + request_hash
  → Return cached response if already completed
  → Prevent accidental duplicate charges/holds
```

---

## Performance Optimization

1. **Stored Procedures**: SQL-level filtering, joins, aggregations
2. **Indexes**: Key indexes on frequently filtered columns (event_id, status, created_at)
3. **Redis Caching**: Event details, seat inventory snapshots
4. **Connection Pooling**: Npgsql built-in
5. **Pagination**: All list endpoints paginated (20 items default)
6. **Distributed Locking**: Redis prevents race conditions on seat inventory
7. **Async/Await**: All I/O operations non-blocking
8. **Soft Deletes**: Logical deletes avoid physical deletions (hot data)

---

## Integration Points

### External Services
- **Payment Gateway**: VNPay, Momo (swap implementations)
- **Email Service**: Send confirmation tickets (stub for now)
- **SMS Provider**: Send hold expiry alerts (future)

### Client Application
- **React SPA**: Connects to `/api/client/*` endpoints
- **WebSocket**: Connects to SignalR hub (`/hubs/ticketing`)
- **JWT Token**: Stored in localStorage, sent with every request


