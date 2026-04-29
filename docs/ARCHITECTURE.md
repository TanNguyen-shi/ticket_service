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
- **Idempotency**: Prevent double-hold via idempotency key (scoped per event+customer) + request hash validation
- **Entities**: EventSeatInventory, SeatHold, SeatHoldItem, IdempotencyRequest
- **Use Cases**: IBookingUseCases (HoldSeat, Checkout, ReleaseHold, ReleaseExpiredHolds)
- **Background Job**: SeatHoldExpiryBackgroundService — runs every 60s, releases holds expired > 10 min
- **Concurrency**: Optimistic locking on EventSeatInventory via `version` field

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

### 2. Client Holds Seat (Complex Flow)

```
BookingController.HoldSeat(BookingHoldSeatRequest)
  ├─ Extract user_id from JWT
  └─ Call BookingUseCases.HoldSeat(request, customerId, cancellationToken)
    │
    ├─ [IDEMPOTENCY GUARD]
    │  ├─ Build scoped key: "hold-evt{event_id}_cust{customerId}_{client_uuid}"
    │  ├─ Hash entire request payload (SHA-256) → request_hash
    │  ├─ Lookup existing IdempotencyRequest by key
    │  │
    │  ├─ If exists:
    │  │  ├─ Hash mismatch → return error (key reused for different payload)
    │  │  ├─ status=completed + snapshot valid → return cached response
    │  │  ├─ status=processing + not expired → return "already processing" error
    │  │  └─ status=failed / processing+expired / completed+broken snapshot
    │  │       → UpdateAsync existing record → reset to status=processing
    │  │
    │  └─ If not exists → InsertAsync new record with status=processing, expired_at=+10min
    │
    ├─ [PROCESS HOLD — ProcessHoldSeat()]
    │  ├─ Open connection
    │  ├─ Query EventSeatInventory for requested seat_ids (verify all available)
    │  ├─ Begin transaction
    │  ├─ Insert SeatHold (hold_code, event_id, customer_id, status=active, expires=+10min)
    │  ├─ For each seat:
    │  │  ├─ Insert SeatHoldItem (hold_id, seat_id, price_at_hold, seat_label_snapshot, ...)
    │  │  └─ UpdateHoldAsync on EventSeatInventory (available→held, optimistic lock via version)
    │  └─ Commit transaction
    │
    ├─ Update IdempotencyRequest → status=completed + response_snapshot (JSON)
    │                           or status=failed on error
    └─ Return BookingHoldSeatDto with hold_id, hold_expires_at, held_seats[]
```

### 3. Client Release Hold (Manual Cancel)

```
BookingController.ReleaseHold(holdId)
  ├─ Extract user_id from JWT
  └─ Call BookingUseCases.ReleaseHoldAsync(holdId, customerId, cancellationToken)
    ├─ Validate: hold exists, customer_id matches, status=active
    ├─ Load SeatHoldItems for the hold
    ├─ Begin transaction
    ├─ For each item: UpdateReleaseAsync on EventSeatInventory (held→available, version++)
    ├─ UpdateStatusByHoldIdAsync on SeatHoldItem → status=released (bulk)
    ├─ UpdateAsync on SeatHold → status=released, released_at=now, release_reason="Khách hàng huỷ"
    └─ Commit → return ReleaseHoldResponseDto
```

### 4. Client Checkout (After Hold)

Payment is currently **mock** — no external gateway is called. Checkout creates order + tickets immediately in `paid` status.

```
BookingController.Checkout(CheckoutRequest)
  ├─ Validate: customerId, hold_id > 0
  ├─ Load SeatHold (verify owner, status=active, not expired)
  ├─ Load SeatHoldItems
  ├─ Begin transaction
  ├─ Insert TicketOrder (order_code, total_amount, order_status=paid, paid_at=now)
  ├─ For each SeatHoldItem:
  │  ├─ Insert TicketOrderItem (order_id, seat, unit_price, item_status=paid)
  │  ├─ UpdateOrderAsync on EventSeatInventory (held→sold)
  │  └─ Insert Ticket (ticket_code, customer_id, seat_label_snapshot, ticket_status=active)
  ├─ UpdateAsync SeatHold → status=converted
  ├─ UpdateStatusByHoldIdAsync SeatHoldItem → status=converted (bulk)
  ├─ Insert PaymentTransaction (payment_provider=mock, payment_status=success)
  └─ Commit → return CheckoutResponseDto with order_id, order_code, final_amount, tickets[]
```

### 5. Seat Hold Auto-Expire (Background Service)

`SeatHoldExpiryBackgroundService` runs as a singleton `IHostedService`, iterating every **60 seconds**.
Each iteration creates a fresh DI scope to resolve the scoped `IBookingUseCases`.

```
SeatHoldExpiryBackgroundService.ExecuteAsync (every 60s)
  └─ Create DI scope → resolve IBookingUseCases
     └─ BookingUseCases.ReleaseExpiredHoldsAsync()
       ├─ Query seat_hold WHERE status='active' AND hold_expires_at < now()
       └─ For each expired hold (in isolation):
            BookingUseCases.ProcessExpiredRelease(holdId)
              ├─ [OUTSIDE transaction] Read SeatHold + SeatHoldItems
              ├─ Guard: skip if hold is no longer active (already released by another path)
              ├─ Begin transaction
              ├─ For each item: UpdateReleaseAsync on EventSeatInventory (held→available)
              ├─ UpdateStatusByHoldIdAsync SeatHoldItem → status=released (bulk)
              ├─ UpdateAsync SeatHold → status=released, release_reason="Hết hạn (10 phút)"
              ├─ Commit (clears connection via CloseAsync)
              └─ On error: RollbackAsync (clears connection) → throw
                   └─ Caller catches, continues with next hold
```

**Key design decision**: reads happen *outside* the try/catch, transaction wraps only writes. `RollbackAsync` calls `CloseAsync` which clears `DapperContextAccessor`, so the next iteration always gets a clean connection even after a failure.

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


