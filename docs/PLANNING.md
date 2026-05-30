# Implementation Planning

## Mục tiêu phase chuyên đề

Làm thông luồng **end-to-end** để demo:

```
Admin: Venue/Section/Seat → Event/Zone/Price → Gán ZoneSection → Generate Inventory → set on_sale
Client: Đăng ký → Đăng nhập → Xem sự kiện → Hold seat → Checkout → Xem vé
```

---

## Trạng thái hiện tại

### ✅ Backend đã hoạt động

| Module | Admin | Client |
|--------|-------|--------|
| Auth | Login (JWT) | Register + Login |
| Event | CRUD | Featured / Trending / Upcoming / Search / Detail |
| Venue / Section / Seat | CRUD | — |
| EventZone + EventZonePrice | CRUD | — |
| SysUser / SysRole / SysUserRole | CRUD | — |
| Booking | — | HoldSeat (idempotency) + Checkout + ReleaseHold |
| Ticket | — | My Tickets + Detail |
| Background Job | SeatHoldExpiry mỗi 60s | — |

### ❌ Còn thiếu để end-to-end hoạt động

| # | Thiếu | Impact |
|---|-------|--------|
| 1 | **EventZoneSection** — insert + list | Admin không map được zone → section |
| 2 | **EventSeatInventory** — generate + list | Client không có ghế để chọn |

---

## Item 1: EventZoneSection (chỉ cần insert + list)

### Mục đích

Admin khai báo: zone X của event này dùng section Y của venue.
Không cần update (thay đổi thì xóa rồi tạo lại). Không cần getbyid riêng.

### Stored procs đã có trong DB

| Proc | Dùng cho |
|------|---------|
| `event_zone_section_insert` | Tạo mapping |
| `event_zone_section_delete` | Xóa mapping |
| `event_zone_section_getpagedlist` | List theo event_id / zone_id |
| `event_zone_section_getbyeventzoneid` | Lấy theo zone |
| `event_zone_section_geteventid` | Lấy theo event ⚠️ tên khác với repo |

> **⚠️ Naming mismatch**: DB có `event_zone_section_geteventid`, repo `event_zone_section_functions.sql` gọi là `getbyeventid`. Cần thống nhất khi implement C# repository.

### Files cần tạo

```
Ticketing.Domain/Domain/EventZoneSection/
  Interfaces/IEventZoneSectionDomainService.cs   ← mới
  EventZoneSectionDomainService.cs               ← mới

Ticketing.Application/UseCases/Admin/EventZoneSection/
  Interfaces/IEventZoneSectionUseCases.cs        ← mới
  EventZoneSectionUseCases.cs                    ← mới

TicketingSystem/Controllers/Admin/Event/
  EventZoneSectionController.cs                  ← mới

Ticketing.Infrastructure/Repositories/EventZoneSection/
  EventZoneSectionRepository.cs                  ← sửa (thêm GetByEventIdAsync)
```

### API endpoints

```
[Route("api/admin/event-zone-section")]
[Authorize]

POST   insert          body: { event_id, event_zone_id, section_id }
DELETE delete          body: { event_zone_section_id }
GET    getpagedlist    query: event_id, event_zone_id, section_id, pagesize, offset
```

### DI

```csharp
// DomainConfigDI.cs
services.AddScoped<IEventZoneSectionDomainService, EventZoneSectionDomainService>();

// UseCaseConfigureDI.cs
services.AddScoped<IEventZoneSectionUseCases, EventZoneSectionUseCases>();
```

---

## Item 2: EventSeatInventory (generate + read-only)

### Mục đích

- **Generate**: Admin bấm 1 nút → hệ thống tự tạo toàn bộ `event_seat_inventory` cho event dựa trên zone-section-seat đã khai báo.
- **GetPagedList / GetById**: Admin xem trạng thái ghế để monitor (available/held/sold).

Insert/Update/Delete đơn lẻ **không cần** — insert được cover bởi generate, update/delete bypass business logic booking.

### Stored procs đã có trong DB

| Proc | Dùng cho |
|------|---------|
| `event_seat_inventory_getbyid` | Xem chi tiết 1 ghế |
| `event_seat_inventory_getpagedlist` | List ghế theo event/zone/status |
| `event_seat_inventory_getbyseatids` | BookingUseCases.HoldSeat (đã dùng) |
| `event_seat_inventory_update_hold` | BookingUseCases.HoldSeat (đã dùng) |
| `event_seat_inventory_updateorder` | BookingUseCases.Checkout (đã dùng) |
| `event_seat_inventory_update_release` | ReleaseHold + Background job (đã dùng) |

> ❌ **Chưa có**: `event_seat_inventory_generate` — cần tạo mới.

### Stored proc cần tạo: `event_seat_inventory_generate`

```sql
CREATE OR REPLACE FUNCTION ticketing.event_seat_inventory_generate(p_event_id bigint)
RETURNS refcursor AS $$
DECLARE
    v_out refcursor := 'esi_generate_' || replace(gen_random_uuid()::text, '-', '_');
    v_count int;
BEGIN
    INSERT INTO ticketing.event_seat_inventory
        (event_id, seat_id, event_zone_id, seat_status, base_price, version, updated_at)
    SELECT
        ezs.event_id,
        vs.seat_id,
        ezs.event_zone_id,
        'available',
        COALESCE(
            (SELECT price FROM ticketing.event_zone_price
             WHERE event_zone_id = ezs.event_zone_id
               AND is_active = true AND is_deleted = false
             ORDER BY created_at DESC LIMIT 1),
            0
        ),
        1,
        now()
    FROM ticketing.event_zone_section ezs
    INNER JOIN ticketing.venue_seat vs
        ON vs.section_id = ezs.section_id
       AND vs.status = 'active'
       AND vs.is_deleted = false
    WHERE ezs.event_id = p_event_id
      AND ezs.is_deleted = false
    ON CONFLICT (event_id, seat_id) DO NOTHING;

    GET DIAGNOSTICS v_count = ROW_COUNT;

    OPEN v_out FOR SELECT v_count AS generated_count;
    RETURN v_out;
END;
$$ LANGUAGE plpgsql;
```

**Thiết kế decisions:**
- `ON CONFLICT DO NOTHING` → idempotent, gọi nhiều lần an toàn
- Chỉ insert ghế chưa có, không touch ghế đang `held`/`sold`
- `base_price` = giá active gần nhất của zone (nếu chưa có price → 0, admin cần khai báo price trước)
- Trả về `generated_count` (số ghế mới tạo, = 0 nếu đã generate rồi)

### Files cần tạo/sửa

```
database/functions/
  event_seat_inventory_generate.sql              ← mới

Ticketing.Infrastructure/Repositories/EventSeatInventory/
  EventSeatInventoryRepository.cs               ← thêm GenerateAsync

Ticketing.Domain/Domain/EventSeatInventory/
  Interfaces/IEventSeatInventoryDomainService.cs ← thêm GenerateAsync
  EventSeatInventoryDomainService.cs             ← implement GenerateAsync

Ticketing.Application/UseCases/Admin/EventSeatInventory/
  Interfaces/IEventSeatInventoryUseCases.cs      ← thêm GenerateAsync
  EventSeatInventoryUseCases.cs                  ← implement GenerateAsync

TicketingSystem/Controllers/Admin/Event/
  EventSeatInventoryController.cs               ← mới
```

**DI không cần chỉnh** — `EventSeatInventoryDomainService`, `EventSeatInventoryUseCases`, `EventSeatInventoryRepository` đã registered.

### API endpoints

```
[Route("api/admin/event-seat-inventory")]
[Authorize]

POST  generate/{eventId:long}     ← tạo inventory cho toàn event, idempotent
GET   getpagedlist                query: event_id, event_zone_id, seat_status, pagesize, offset
GET   getbyid                     query: event_seat_inventory_id
```

### Response format

```json
// POST generate/{eventId} — success
{
  "issuccess": true,
  "status": "success",
  "message": "Khởi tạo inventory thành công: 250 ghế",
  "data": 250
}

// POST generate/{eventId} — already generated
{
  "issuccess": true,
  "status": "success",
  "message": "Inventory đã tồn tại, không có ghế mới nào được tạo",
  "data": 0
}
```

---

## Thứ tự implement

```
1. event_seat_inventory_generate.sql    (SQL)
2. EventSeatInventoryRepository         (thêm GenerateAsync)
3. IEventSeatInventoryDomainService     (thêm signature)
4. EventSeatInventoryDomainService      (implement)
5. IEventSeatInventoryUseCases          (thêm signature)
6. EventSeatInventoryUseCases           (implement)
7. EventSeatInventoryController         (tạo mới)
8. EventZoneSectionDomainService + Interface
9. EventZoneSectionUseCases + Interface
10. EventZoneSectionController
11. DI registration cho EventZoneSection
```

---

## Bugs cần fix (trước khi test end-to-end)

| Bug | File | Mô tả |
|-----|------|-------|
| `order_item_code` không tồn tại | `event_seat_inventory_getbyid`, `getpagedlist` stored proc | Xóa dòng `toi.order_item_code` |
| Alias `u` không tồn tại | `payment_transaction_getpagedlist` stored proc | Đổi thành alias `c` |
| `su.fullname` sai tên cột | `event_publish_log_getbyid`, `getpagedlist` stored proc | Đổi thành `su.full_name` |
| Naming mismatch `geteventid` | `EventZoneSectionRepository` | Thống nhất tên proc với C# |

---

## Luồng end-to-end sau khi hoàn thành

```
Admin setup:
  1. Tạo Venue → VenueSection → VenueSeat
  2. Tạo Event → EventZone → EventZonePrice
  3. POST /api/admin/event-zone-section/insert  ← Item 1
     (gán section của venue vào zone của event)
  4. POST /api/admin/event-seat-inventory/generate/{eventId}  ← Item 2
     (khởi tạo inventory cho toàn event)
  5. Update Event status = "on_sale"

Client flow:
  1. POST /api/client/auth/register
  2. POST /api/client/auth/login
  3. GET  /api/client/event/featured (hoặc search)
  4. GET  /api/client/event/{eventId}  ← xem ghế + trạng thái
  5. POST /api/client/booking/hold-seat
  6. POST /api/client/booking/checkout
  7. GET  /api/client/ticket/my-tickets
```
