# SeatHold Module - Implementation Checklist

## ✅ Phase 1: Infrastructure Layer (COMPLETED)

### Entities
- [x] SeatHoldEntity.cs - Seat hold master record
- [x] SeatHoldItemEntity.cs - Held seats collection

### DTOs - Request Models
- [x] SeatHoldInsertDto - Create new hold
- [x] SeatHoldUpdateDto - Update hold
- [x] SeatHoldCheckDto - Duplicate check
- [x] SeatHoldDeleteDto - Delete hold
- [x] SeatHoldGetByIdDto - Retrieve single
- [x] SeatHoldGetPagedListDto - Paginated search
- [x] SeatHoldItemInsertDto - Create hold item
- [x] SeatHoldItemUpdateDto - Update hold item
- [x] SeatHoldItemCheckDto - Duplicate check
- [x] SeatHoldItemDeleteDto - Delete item
- [x] SeatHoldItemGetByIdDto - Retrieve single item
- [x] SeatHoldItemGetPagedListDto - Paginated item search

### DTOs - Response Models
- [x] SeatHoldResponseDto - Single hold response
- [x] SeatHoldPagedDto - Paginated holds response
- [x] SeatHoldItemResponseDto - Single item response
- [x] SeatHoldItemPagedDto - Paginated items response

### Repositories
- [x] ISeatHoldRepository interface
- [x] SeatHoldRepository implementation
- [x] ISeatHoldItemRepository interface
- [x] SeatHoldItemRepository implementation
- [x] ISeatHoldUnitOfWork interface
- [x] SeatHoldUnitOfWork implementation

### SQL Functions (16 total)
- [x] seat_hold_check - Check duplicate hold codes
- [x] seat_hold_insert - Insert new hold
- [x] seat_hold_update - Update hold
- [x] seat_hold_delete - Delete hold
- [x] seat_hold_getbyid - Get single hold with relations
- [x] seat_hold_getpagedlist - Paginated list with filters
- [x] seat_hold_getbyeventid - Get holds by event
- [x] seat_hold_getbyuserid - Get holds by user
- [x] seat_hold_item_check - Check duplicate items
- [x] seat_hold_item_insert - Insert hold item
- [x] seat_hold_item_update - Update hold item
- [x] seat_hold_item_delete - Delete hold item
- [x] seat_hold_item_getbyid - Get single item with relations
- [x] seat_hold_item_getpagedlist - Paginated item list
- [x] seat_hold_item_getbyholdid - Get items by hold ID

### Documentation
- [x] SEATHOLD_MODULE_IMPLEMENTATION.md
- [x] SEATHOLD_DI_SETUP.md
- [x] SEATHOLD_IMPLEMENTATION_SUMMARY.md
- [x] SEATHOLD_MODULE_IMPLEMENTATION_CHECKLIST.md (this file)

---

## ⏳ Phase 2: Database Setup (TODO - Next Step)

### Prerequisites
- [ ] PostgreSQL server running
- [ ] Database `ticketing_db` exists
- [ ] Database user with CREATE FUNCTION privilege

### Tasks
1. [ ] Execute SQL functions script
   ```bash
   psql -U postgres -d ticketing_db -f database/functions/seat_hold_functions.sql
   ```

2. [ ] Verify functions created
   ```sql
   SELECT routine_name FROM information_schema.routines 
   WHERE routine_schema = 'ticketing' AND routine_name LIKE 'seat_hold%'
   ORDER BY routine_name;
   ```

3. [ ] Test sample function
   ```sql
   BEGIN;
   SELECT ticketing.seat_hold_check('TEST001', 0);
   FETCH ALL IN "seat_hold_check";
   COMMIT;
   ```

---

## ⏳ Phase 3: Dependency Injection Setup (TODO - Next Step)

### File: Ticketing.Infrastructure/Configurations/InfrastructureConfigDI.cs

- [ ] Add using statement: `using Ticketing.Infrastructure.Repositories.SeatHold;`
- [ ] Add using statement: `using Ticketing.Infrastructure.Persistence.Helpers;`
- [ ] Register ISeatHoldUnitOfWork in ConfigureInfrastructureDI() method

```csharp
services.AddScoped<ISeatHoldUnitOfWork>(provider =>
{
    var dapperContext = provider.GetRequiredService<DapperContext>();
    var contextAccessor = provider.GetRequiredService<DapperContextAccessor>();
    var dapperHelper = provider.GetRequiredService<IDapperProcedureHelper>();
    
    return new SeatHoldUnitOfWork(dapperContext, contextAccessor, dapperHelper);
});
```

- [ ] Test DI resolution in unit tests or application startup

---

## ⏳ Phase 4: Domain Services (OPTIONAL)

### Create Domain Service for Business Logic

File: `Ticketing.Domain/Services/SeatHoldDomainService.cs`

```csharp
public interface ISeatHoldDomainService
{
    Task<bool> CanHoldSeatsAsync(long eventId, long userId, CancellationToken cancellationToken);
    Task<SeatHoldResult> CreateSeatHoldAsync(CreateSeatHoldCommand cmd, CancellationToken cancellationToken);
    Task<bool> ReleaseSeatHoldAsync(long holdId, string reason, CancellationToken cancellationToken);
    Task<bool> ExpireSeatHoldAsync(long holdId, CancellationToken cancellationToken);
    Task<bool> ConvertHoldToOrderAsync(long holdId, long orderId, CancellationToken cancellationToken);
}
```

Responsibilities:
- [ ] Validate business rules (seat availability, user eligibility)
- [ ] Check hold expiration
- [ ] Manage hold state transitions
- [ ] Handle concurrent access

---

## ⏳ Phase 5: Application Layer (UseCase Layer) (TODO)

### Create UseCases

Location: `Ticketing.Application/UseCases/Client/Booking/`

- [ ] **HoldSeatsUseCase.cs**
  ```
  Request: HoldSeatsRequest { event_id, user_id, seat_ids, zone_id }
  Response: BookingHoldSeatDto { hold_id, hold_code, hold_expires_at, held_seats[] }
  Logic: Create seat hold + hold items with transaction
  ```

- [ ] **ReleaseHoldUseCase.cs**
  ```
  Request: ReleaseHoldRequest { hold_id, release_reason }
  Response: SimpleResponseDto { success, message }
  Logic: Update hold status to 'released'
  ```

- [ ] **GetUserHoldsUseCase.cs**
  ```
  Request: GetUserHoldsRequest { user_id, page, size }
  Response: PagedResult<SeatHoldPagedDto>
  Logic: Query user's active holds
  ```

- [ ] **GetHoldDetailsUseCase.cs**
  ```
  Request: GetHoldDetailsRequest { hold_id }
  Response: SeatHoldResponseDto (with held_seats array)
  Logic: Get hold + all items with seat details
  ```

- [ ] **CheckHoldExpiredUseCase.cs**
  ```
  Request: CheckHoldExpiredRequest { hold_id }
  Response: HoldStatusDto { is_expired, time_remaining }
  Logic: Check hold expiration and auto-release if needed
  ```

---

## ⏳ Phase 6: Presentation Layer (API Controllers) (TODO)

### Create Controllers

Location: `TicketingSystem/Controllers/Client/`

**File: BookingController.cs**

API Endpoints:
- [ ] `POST /api/client/booking/hold-seats` → HoldSeatsUseCase
  - Request: { event_id, seat_ids, zone_id }
  - Response: { hold_id, hold_code, hold_expires_at, held_seats[] }

- [ ] `POST /api/client/booking/release-hold/{hold_id}` → ReleaseHoldUseCase
  - Request: { release_reason }
  - Response: { success, message }

- [ ] `GET /api/client/booking/my-holds` → GetUserHoldsUseCase
  - Query: page=1&size=20
  - Response: PagedResult<SeatHoldPagedDto>

- [ ] `GET /api/client/booking/holds/{hold_id}` → GetHoldDetailsUseCase
  - Response: SeatHoldResponseDto with held_seats[]

- [ ] `GET /api/client/booking/holds/{hold_id}/status` → CheckHoldExpiredUseCase
  - Response: HoldStatusDto

---

## ⏳ Phase 7: Admin Layer (Optional) (TODO)

### Admin Endpoints

Location: `TicketingSystem/Controllers/Admin/`

**File: AdminBookingController.cs**

- [ ] `GET /api/admin/booking/holds` - List all holds (paginated)
- [ ] `GET /api/admin/booking/holds/event/{event_id}` - Holds by event
- [ ] `GET /api/admin/booking/holds/user/{user_id}` - Holds by user
- [ ] `PATCH /api/admin/booking/holds/{hold_id}/status` - Change hold status
- [ ] `POST /api/admin/booking/holds/{hold_id}/force-release` - Force release hold

---

## ⏳ Phase 8: Scheduled Jobs (OPTIONAL)

### Background Services

Location: `TicketingSystem/BackgroundServices/`

**File: ExpiredHoldCleanupService.cs**

- [ ] Scheduled to run every minute
- [ ] Find expired holds (hold_expires_at < now())
- [ ] Update status from 'active' to 'expired'
- [ ] Release event_seat_inventory back to available
- [ ] Log cleanup operations

```csharp
// Pseudo-code
public class ExpiredHoldCleanupService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Find expired holds
            var expiredHolds = await GetExpiredHolds();
            
            // For each hold, release seat inventory
            foreach (var hold in expiredHolds)
            {
                await ReleaseHoldAsync(hold);
            }
            
            // Wait 1 minute
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
```

---

## ⏳ Phase 9: Testing (TODO)

### Unit Tests
- [ ] SeatHoldRepository tests
- [ ] SeatHoldItemRepository tests
- [ ] SeatHoldDomainService tests

### Integration Tests
- [ ] HoldSeatsUseCase test
- [ ] ReleaseHoldUseCase test
- [ ] GetUserHoldsUseCase test
- [ ] Transaction rollback on error

### Performance Tests
- [ ] Concurrent hold requests
- [ ] Batch seat availability check
- [ ] Pagination with large datasets

---

## ⏳ Phase 10: API Documentation (TODO)

### OpenAPI/Swagger

- [ ] Document HoldSeats endpoint
- [ ] Document ReleaseHold endpoint
- [ ] Document GetUserHolds endpoint
- [ ] Document GetHoldDetails endpoint
- [ ] Document status codes and error responses

### API Examples

- [ ] cURL examples for each endpoint
- [ ] Postman collection
- [ ] API response examples
- [ ] Error response examples

---

## 📊 Implementation Progress

### Current Status: **Phase 1 Completed (Infrastructure)**

```
Phase 1: Infrastructure Layer        ████████████████████ 100% ✓
Phase 2: Database Setup              ░░░░░░░░░░░░░░░░░░░░   0% ⏳
Phase 3: Dependency Injection        ░░░░░░░░░░░░░░░░░░░░   0% ⏳
Phase 4: Domain Services             ░░░░░░░░░░░░░░░░░░░░   0% ⏳ (Optional)
Phase 5: Application Layer (UseCases)░░░░░░░░░░░░░░░░░░░░   0% ⏳
Phase 6: Presentation Layer (API)    ░░░░░░░░░░░░░░░░░░░░   0% ⏳
Phase 7: Admin Layer                 ░░░░░░░░░░░░░░░░░░░░   0% ⏳ (Optional)
Phase 8: Scheduled Jobs              ░░░░░░░░░░░░░░░░░░░░   0% ⏳ (Optional)
Phase 9: Testing                     ░░░░░░░░░░░░░░░░░░░░   0% ⏳
Phase 10: Documentation              ░░░░░░░░░░░░░░░░░░░░   0% ⏳

Overall: ████░░░░░░ 10% Complete
```

---

## 🚀 Recommended Next Steps

1. **Immediate (Today)**
   - [ ] Execute SQL functions against database
   - [ ] Add DI registration to InfrastructureConfigDI.cs
   - [ ] Run application and verify DI resolves ISeatHoldUnitOfWork

2. **This Week**
   - [ ] Create HoldSeatsUseCase
   - [ ] Create BookingController with hold-seats endpoint
   - [ ] Manual API testing

3. **Next Week**
   - [ ] Create remaining UseCases (Release, Get, Status)
   - [ ] Create Admin endpoints
   - [ ] Write integration tests

4. **Following Week**
   - [ ] Create background job for expired hold cleanup
   - [ ] Performance testing
   - [ ] API documentation

---

## 📝 Files Summary

### Total Files Created: 15

```
Entities:                   2 files
DTOs (Request):             2 files
DTOs (Response):            2 files
Repositories:               3 files
SQL Functions:              1 file
Documentation:              5 files
───────────────────────────────────
Total:                     15 files
```

### File Sizes

```
SeatHoldEntity.cs                    ~0.5 KB
SeatHoldItemEntity.cs                ~0.5 KB
SeatHoldRequestDto.cs                ~2 KB
SeatHoldItemRequestDto.cs            ~2 KB
SeatHoldResponseDto.cs               ~2 KB
SeatHoldItemResponseDto.cs           ~3 KB
SeatHoldRepository.cs                ~2 KB
SeatHoldItemRepository.cs            ~1.5 KB
SeatHoldUnitOfWork.cs                ~1 KB
seat_hold_functions.sql              ~32 KB
SEATHOLD_MODULE_IMPLEMENTATION.md    ~12 KB
SEATHOLD_DI_SETUP.md                 ~8 KB
SEATHOLD_IMPLEMENTATION_SUMMARY.md   ~20 KB
SEATHOLD_MODULE_IMPLEMENTATION.md    ~12 KB
───────────────────────────────────
Total:                              ~100 KB
```

---

## 🔗 Related Documentation

- Main Implementation Guide: `SEATHOLD_MODULE_IMPLEMENTATION.md`
- DI Setup Guide: `SEATHOLD_DI_SETUP.md`
- Summary: `SEATHOLD_IMPLEMENTATION_SUMMARY.md`
- This Checklist: `SEATHOLD_MODULE_IMPLEMENTATION_CHECKLIST.md`

---

## 📞 Quick Links

### Repository Structure
```
Ticketing.Infrastructure/
├── Entities/SeatHold/
├── DTOs/Client/SeatHold/
└── Repositories/SeatHold/

database/
└── functions/seat_hold_functions.sql
```

### Key Classes
- `SeatHoldEntity` - Master hold record
- `SeatHoldItemEntity` - Individual seat hold
- `ISeatHoldRepository` - Hold data access
- `ISeatHoldItemRepository` - Hold item data access
- `ISeatHoldUnitOfWork` - Transaction management

### Entry Points for Next Phase
1. `InfrastructureConfigDI.cs` - Add DI registration
2. `PostgreSQL` - Execute SQL functions
3. `Ticketing.Application/UseCases/` - Create use cases
4. `TicketingSystem/Controllers/Client/` - Create API endpoints

---

**Last Updated**: 2026-04-05
**Status**: ✅ Infrastructure Layer Complete
**Next Phase**: Phase 2 - Database Setup

