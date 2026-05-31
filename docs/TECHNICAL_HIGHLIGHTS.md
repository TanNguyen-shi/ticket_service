# Điểm Kỹ Thuật Nổi Bật — Hệ Thống Đặt Vé Sự Kiện

Tài liệu này tóm tắt các kỹ thuật và thuật toán quan trọng được áp dụng trong hệ thống, phục vụ cho báo cáo chuyên đề.

---

## 1. Kiến Trúc Tổng Thể — Clean Architecture

Hệ thống được thiết kế theo **Clean Architecture** với 4 tầng độc lập, mỗi tầng chỉ phụ thuộc vào tầng bên trong:

```
┌─────────────────────────────────────┐
│  Presentation (Controller + JWT)    │  ← HTTP request/response
├─────────────────────────────────────┤
│  Application (UseCase)              │  ← Điều phối luồng xử lý
├─────────────────────────────────────┤
│  Domain (DomainService + UnitOfWork)│  ← Nghiệp vụ + Transaction
├─────────────────────────────────────┤
│  Infrastructure (Repository + Redis)│  ← Dapper + PostgreSQL + Redis
└─────────────────────────────────────┘
```

**Lợi ích**: Tách biệt rõ trách nhiệm, dễ test từng tầng, thay thế infrastructure (ví dụ đổi từ Dapper sang EF Core) mà không ảnh hưởng nghiệp vụ.

---

## 2. Vấn Đề Cốt Lõi: Race Condition & Overselling

### 2.1 Bài Toán

Trong hệ thống bán vé, kịch bản nguy hiểm nhất là **2 người dùng cùng lúc đặt 1 ghế**:

```
Thời điểm T1: User A đọc ghế G01 → status = "available", version = 3
Thời điểm T1: User B đọc ghế G01 → status = "available", version = 3
Thời điểm T2: User A cập nhật G01 → held (version 3 → 4) ✅ COMMIT
Thời điểm T2: User B cập nhật G01 → held WHERE version = 3 → 0 rows affected ❌
```

Nếu không có cơ chế kiểm soát, cả hai đều thành công → **1 ghế bán cho 2 người** (overselling).

### 2.2 Giải Pháp: Optimistic Locking với `version`

Bảng `event_seat_inventory` có cột `version INT DEFAULT 1`. Stored procedure `event_seat_inventory_update_hold` kiểm tra version trước khi cập nhật:

```sql
UPDATE ticketing.event_seat_inventory
SET seat_status = 'held',
    current_hold_id = p_current_hold_id,
    version = version + 1,          -- tăng version sau mỗi lần cập nhật
    updated_at = now()
WHERE event_seat_inventory_id = p_id
  AND seat_status = 'available'     -- guard: chỉ cập nhật khi còn trống
  AND version = p_version;          -- guard: phiên bản phải khớp
```

Phía C# kiểm tra kết quả:

```csharp
var updateSuccess = await repository.UpdateHoldAsync(new {
    event_seat_inventory_id = inventory.event_seat_inventory_id,
    version = inventory.version   // version đọc được từ DB trước đó
}, cancellationToken);

if (!updateSuccess)
    throw new Exception($"Ghế {inventory.seat_label} vừa được người khác đặt");
    // → Transaction rollback toàn bộ
```

**Kết quả**: Người đến sau nhận exception, transaction rollback — ghế vẫn chỉ thuộc về 1 người.

---

## 3. Cơ Chế Idempotency — Chống Double-Click & Retry

### 3.1 Vấn Đề

Người dùng bấm "Giữ chỗ" 2 lần liên tiếp (double-click), hoặc request bị timeout rồi tự retry → hệ thống có thể tạo 2 hold cho cùng 1 yêu cầu.

### 3.2 Thiết Kế

Client sinh 1 UUID trước mỗi lần bấm giữ chỗ (idempotency_key). Server scope key theo `event_id + customer_id` để chống dùng chung key giữa các khách hàng:

```
idempotency_key = "hold-evt{event_id}_cust{customerId}_{client_uuid}"
```

Server hash toàn bộ request body bằng SHA-256 để phát hiện key bị tái dùng với payload khác:

```
request_hash = SHA256(JSON.Serialize(request))
```

### 3.3 State Machine của Idempotency Record

```
                        ┌─────────────┐
             Tạo mới →  │ processing  │
                        └──────┬──────┘
                               │
               ┌───────────────┼───────────────┐
               ▼               ▼               ▼
          ┌─────────┐   ┌──────────┐   ┌────────────┐
          │completed│   │  failed  │   │  expired   │
          └─────────┘   └──────────┘   └────────────┘
```

| Trạng thái | Hành động khi nhận request trùng |
|-----------|----------------------------------|
| `processing` + chưa hết hạn | Trả về lỗi "đang xử lý" |
| `completed` + snapshot hợp lệ | Trả về response đã cache (không xử lý lại) |
| `failed` / `processing` hết hạn / snapshot hỏng | Reset về `processing`, xử lý lại |
| Key trùng nhưng hash khác | Báo lỗi "key đã dùng cho payload khác" |

---

## 4. Transaction Management — UnitOfWork Pattern

### 4.1 Vấn Đề

Quy trình hold seat gồm nhiều bước ghi DB (tạo hold, tạo hold items, cập nhật inventory từng ghế). Nếu bước giữa thất bại, phải rollback toàn bộ để tránh trạng thái không nhất quán.

### 4.2 Thiết Kế

`DapperContextAccessor` (Scoped) lưu connection + transaction dùng chung trong suốt 1 request. Mọi repository trong cùng request đều dùng chung 1 transaction này:

```csharp
// Domain Service mở và quản lý transaction
await unitOfWork.OpenAsync();
await unitOfWork.BeginTransactionAsync();
try
{
    await holdRepo.InsertAsync(...);          // Tạo hold
    foreach (var seat in seats)
    {
        await holdItemRepo.InsertAsync(...);  // Tạo hold item
        await inventoryRepo.UpdateHoldAsync(...); // Cập nhật inventory
    }
    await unitOfWork.CommitAsync();           // Commit toàn bộ nếu thành công
}
catch
{
    await unitOfWork.RollbackAsync();         // Rollback toàn bộ nếu có lỗi
}
```

**Nguyên tắc**: Đọc dữ liệu ngoài transaction (để tối ưu hiệu năng), ghi trong transaction (để đảm bảo tính toàn vẹn).

---

## 5. Background Job — Tự Động Trả Ghế Hết Hạn

### 5.1 Thiết Kế

`SeatHoldExpiryBackgroundService` là một `IHostedService` chạy mỗi **60 giây**:

```
Mỗi 60 giây:
  └─ Tạo DI scope mới (tránh dùng chung Scoped service với request)
     └─ Query: seat_hold WHERE status = 'active' AND hold_expires_at < NOW()
        └─ Với mỗi hold hết hạn (xử lý độc lập):
             ├─ Đọc thông tin hold + hold items (ngoài transaction)
             ├─ Guard: bỏ qua nếu hold không còn active
             ├─ Mở transaction
             ├─ Cập nhật inventory: held → available (version++)
             ├─ Cập nhật hold items: active → released
             ├─ Cập nhật hold: active → released
             └─ Commit / Rollback (chỉ ảnh hưởng hold hiện tại)
```

### 5.2 Điểm Kỹ Thuật Quan Trọng

**Isolation per hold**: Mỗi hold hết hạn được xử lý trong transaction riêng biệt. Nếu release một hold bị lỗi, các hold khác vẫn được xử lý bình thường.

**Scoped service trong Singleton**: Background service là Singleton nhưng cần Scoped dependencies (Repository). Giải pháp: dùng `IServiceScopeFactory` để tạo scope mới mỗi lần chạy:

```csharp
using var scope = _serviceScopeFactory.CreateScope();
var bookingUseCases = scope.ServiceProvider.GetRequiredService<IBookingUseCases>();
```

**RollbackAsync = CloseAsync**: Khi rollback, connection bị đóng và xóa khỏi `DapperContextAccessor`. Vòng lặp tiếp theo luôn bắt đầu với connection sạch.

---

## 6. Chiến Lược Cache — Redis

### 6.1 Cache Event Detail + Seat Status

Dữ liệu event (tên, mô tả, zone, giá) được cache với TTL **5 phút**. Tuy nhiên trạng thái ghế (available/held/sold) thay đổi real-time nên **không được cache** — mỗi request đọc thẳng từ DB:

```
GET /api/client/event/detail?event_id=X
  ├─ Cache hit: Lấy event + zones + seats từ Redis (5 min TTL)
  │   └─ Nhưng vẫn gọi DB để lấy seat_status thực tế → PopulateSeatStatusAsync()
  └─ Cache miss: Đọc DB → cache lại → lấy seat_status thực tế
```

### 6.2 Version-Based Cache Invalidation

Thay vì xóa từng cache key khi event thay đổi, hệ thống dùng **version key**:

```
ticketing:event:client:version = "v-{timestamp}"
```

Cache key cho mỗi list được build kèm version:
```
event:client:featured:{version} = [...]
```

Khi admin cập nhật event → chỉ cần thay đổi version key → toàn bộ cache list tự động miss mà không cần xóa từng key.

### 6.3 Cache Keys & TTL

| Key Pattern | TTL | Dữ liệu |
|-------------|-----|---------|
| `event:detail:{eventId}` | 5 phút | Chi tiết event + zones + giá |
| `event:client:featured:{ver}` | 3 phút | Danh sách event nổi bật |
| `event:client:trending:{ver}` | 3 phút | Danh sách event trending |
| `event:client:upcoming:{ver}` | 3 phút | Danh sách event sắp tới |
| `ticketing:event:client:version` | 30 ngày | Version key cho cache list |

---

## 7. Data Access Pattern — Stored Procedures + Refcursor

### 7.1 Không Raw SQL

Toàn bộ truy vấn DB đều thông qua **PostgreSQL Stored Functions**. Không có raw SQL trong code C#.

```
C# Repository → DapperProcedureHelper → ticketing.{table}_{action}() → refcursor → kết quả
```

### 7.2 Refcursor Pattern

PostgreSQL function trả về refcursor thay vì kết quả trực tiếp, cho phép trả nhiều tập kết quả và tránh conflict cursor name khi có concurrent requests:

```sql
DECLARE
    v_out refcursor := 'seat_hold_getbyid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR SELECT ...;
    RETURN v_out;
END;
```

### 7.3 Batch Query — Tránh N+1

`GetEventDetail` thực hiện **1 query cho mỗi loại dữ liệu** thay vì query theo từng zone/section:

```
1 query → tất cả zones của event
1 query → tất cả prices của tất cả zones (batch)
1 query → tất cả zone-section mappings của event
1 query → tất cả seats của tất cả sections (batch)
1 query → tất cả seat statuses (real-time, không cache)
```

Map trong bộ nhớ thay vì gọi DB nhiều lần.

---

## 8. Luồng End-to-End Giữ Chỗ — Tổng Hợp

```
Client bấm "Giữ chỗ"
    │
    ▼
[1] Idempotency Guard
    ├─ Build key: hold-evt{id}_cust{id}_{uuid}
    ├─ Hash payload: SHA-256
    └─ Kiểm tra & tạo record → status = processing
    │
    ▼
[2] Đọc EventSeatInventory (ngoài transaction)
    └─ Kiểm tra tất cả ghế đều status = available
    │
    ▼
[3] BEGIN TRANSACTION
    ├─ INSERT seat_hold → hold_id
    ├─ Với mỗi ghế:
    │   ├─ INSERT seat_hold_item
    │   └─ UPDATE inventory WHERE version = {version đọc được}
    │       ├─ Thành công → version++, status = held
    │       └─ Thất bại (0 rows) → THROW → ROLLBACK toàn bộ
    └─ COMMIT
    │
    ▼
[4] Cập nhật Idempotency → status = completed + cache response
    │
    ▼
Trả về: hold_id, hold_expires_at, held_seats[]
```

**Kết quả**: Với 1000 concurrent requests cho cùng 1 ghế — **đúng 1 request thành công**, 999 request còn lại nhận lỗi rõ ràng.

---

## 9. Tóm Tắt Kỹ Thuật Áp Dụng

| Vấn đề | Kỹ thuật áp dụng | Hiệu quả |
|--------|-----------------|---------|
| Race condition / Overselling | Optimistic Locking (`version` field) | 1 ghế chỉ bán được cho 1 người |
| Double-click / Retry | Idempotency (UUID + SHA-256 hash + state machine) | Không tạo hold trùng |
| Tính toàn vẹn dữ liệu | UnitOfWork + PostgreSQL Transaction | All-or-nothing, không trạng thái lơ lửng |
| Ghế bị giữ vĩnh viễn | Background Job (60s interval, per-hold isolation) | Tự động trả ghế sau 10 phút |
| Hiệu năng đọc | Redis Cache + Version-based Invalidation | Giảm tải DB cho event listing |
| N+1 Query | Batch Query + In-memory mapping | O(k) queries thay vì O(n) |
| Concurrent DB access | Stored Procedures + UUID cursor name | Tránh cursor conflict |
