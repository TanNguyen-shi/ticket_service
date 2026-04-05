# Idempotency Module - Implementation Guide

## Overview

Idempotency Pattern là một giải pháp để xử lý các yêu cầu lặp lại từ Client. Khi người dùng bị lag mạng hoặc bấm nút multiple lần, hệ thống không tạo ra các xử lý trùng lặp mà trả về kết quả từ lần xử lý đầu tiên.

---

## 📊 Entity Structure

### IdempotencyRequestEntity
```csharp
public long idempotency_id               // PK
public string idempotency_key            // Unique key from client (UUID or timestamp-based)
public string request_type               // hold_seats, payment, etc.
public long user_id                      // User making request
public string? request_hash              // Hash of request payload for duplicate detection
public string status                     // processing, completed, failed, expired
public string? response_snapshot         // JSON response stored for replay
public DateTime expired_at               // When idempotency key expires
```

---

## 🔄 How It Works

### Idempotency Flow

```
Client Request (with idempotency_key)
         ↓
┌─────────────────────────────────┐
│ Check Idempotency Request Table │
│ (idempotency_key + request_type)│
└──────────┬──────────────────────┘
           ↓
    ┌──────────────┐
    │ Found?       │
    └──┬───────┬──┘
       │       │
   YES │       │ NO
       │       ↓
       │    ┌─────────────────────────┐
       │    │ Create new record       │
       │    │ status = 'processing'   │
       │    └──────┬──────────────────┘
       │           ↓
       │    ┌─────────────────────────┐
       │    │ Process Request         │
       │    │ (Business Logic)        │
       │    └──────┬──────────────────┘
       │           ↓
       │    ┌─────────────────────────┐
       │    │ Update status           │
       │    │ status = 'completed'    │
       │    │ Store response_snapshot │
       │    └──────┬──────────────────┘
       │           │
       └───────────┤
                   ↓
         ┌──────────────────────┐
         │ Return Response      │
         │ (Fresh or from cache)│
         └──────────────────────┘
```

---

## 📝 Repository Methods

### IIdempotencyRequestRepository

```csharp
// Inherited from IGenericRepository<IdempotencyRequestEntity>:
InsertAsync()           → idempotency_request_insert
UpdateAsync()           → idempotency_request_update
CheckExistAsync()       → idempotency_request_check
DeleteAsync()           → idempotency_request_delete
GetAsync()              → idempotency_request_getbyid
GetPagedAsync()         → idempotency_request_getpagedlist

// Custom Methods:
GetByKeyAndTypeAsync()  → idempotency_request_getbykeyandtype
GetByUserIdAsync()      → idempotency_request_getbyuserid
```

---

## 🎯 Usage Example in UseCase

### Pattern 1: Check Existing Request

```csharp
public async Task<ResponseMessage<EventSeatStatusDto>> HoldSeat(
    BookingHoldSeatRequest request, 
    CancellationToken cancellationToken = default)
{
    try
    {
        // Step 1: Kiểm tra Idempotency (Chống Double-click)
        var idempotencyKey = $"hold-evt{request.event_id}-user{request.user_id}-{request.request_id}";
        
        var existingRequest = await _idempotencyRepository
            .GetByKeyAndTypeAsync<IdempotencyRequestResponseDto>(
                new { 
                    idempotency_key = idempotencyKey,
                    request_type = "hold_seats"
                }, 
                cancellationToken);
        
        // Nếu đã xử lý xong, trả về kết quả cũ
        if (existingRequest != null && existingRequest.status == "completed")
        {
            var cachedResponse = JsonSerializer.Deserialize<EventSeatStatusDto>(
                existingRequest.response_snapshot);
            return new ResponseMessage<EventSeatStatusDto>()
                .MessageSuccess(cachedResponse, "Request already processed");
        }
        
        // Nếu đang xử lý, từ chối xử lý lại
        if (existingRequest != null && existingRequest.status == "processing")
        {
            return new ResponseMessage<EventSeatStatusDto>()
                .MessageError("Request is still processing");
        }
        
        // Step 2: Tạo mới bản ghi idempotency với status = processing
        var idempotencyDto = new IdempotencyRequestInsertDto
        {
            idempotency_key = idempotencyKey,
            request_type = "hold_seats",
            user_id = request.user_id,
            request_hash = ComputeHash(request),
            status = "processing",
            response_snapshot = null,
            expired_at = DateTime.UtcNow.AddMinutes(10)
        };
        
        var idempotencyId = await _idempotencyRepository
            .InsertAsync(idempotencyDto, cancellationToken);
        
        // Step 3: Xử lý request bình thường
        var result = await eventDomain.GetEventSeatStatus(request, cancellationToken);
        
        // Step 4: Cập nhật idempotency record với status = completed
        var updateDto = new IdempotencyRequestUpdateDto
        {
            idempotency_id = long.Parse(idempotencyId ?? "0"),
            idempotency_key = idempotencyKey,
            request_type = "hold_seats",
            user_id = request.user_id,
            request_hash = ComputeHash(request),
            status = "completed",
            response_snapshot = JsonSerializer.Serialize(result.Data),
            expired_at = DateTime.UtcNow.AddMinutes(10)
        };
        
        await _idempotencyRepository.UpdateAsync(updateDto, cancellationToken);
        
        return result;
    }
    catch (Exception e)
    {
        // Cập nhật status = failed nếu xảy ra lỗi
        return new ResponseMessage<EventSeatStatusDto>()
            .MessageError(e.Message);
    }
}

private string ComputeHash(BookingHoldSeatRequest request)
{
    var json = JsonSerializer.Serialize(request);
    using (var sha256 = System.Security.Cryptography.SHA256.Create())
    {
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
        return Convert.ToBase64String(hashedBytes);
    }
}
```

---

## 💾 SQL Functions

### Core Functions (6)
```sql
✓ idempotency_request_check         - Duplicate check
✓ idempotency_request_insert        - Create new record
✓ idempotency_request_update        - Update record
✓ idempotency_request_delete        - Delete record
✓ idempotency_request_getbyid       - Get single record
✓ idempotency_request_getpagedlist  - Paginated search
```

### Custom Functions (2)
```sql
✓ idempotency_request_getbykeyandtype  - Get by key + type (Most important)
✓ idempotency_request_getbyuserid      - Get by user
```

---

## 🔐 Key Features

### 1. Request Hash
- Stores `request_hash` để detect duplicate payloads
- Compute using SHA256 hash của request JSON
- Giúp detect khi client gửi yêu cầu giống hệt nhau

### 2. Response Snapshot
- Stores `response_snapshot` (JSON) để replay kết quả
- Khi client retry, backend trả về kết quả cũ từ snapshot
- Tiết kiệm resources và đảm bảo consistency

### 3. Expiration
- Mỗi idempotency key có `expired_at` TTL
- Sau khi hết hạn, backend có thể xóa bản ghi để tiết kiệm storage
- Thường set 10 phút cho hold_seats, 1 giờ cho payment

### 4. Status Tracking
```
Statuses:
  - processing: Đang xử lý yêu cầu
  - completed: Xử lý xong, có kết quả
  - failed: Xử lý thất bại
  - expired: Hết hạn, không dùng được nữa
```

---

## 🚀 DI Registration

```csharp
// File: InfrastructureConfigDI.cs

services.AddScoped<IIdempotencyRequestRepository>(provider =>
    new IdempotencyRequestRepository(
        provider.GetRequiredService<IDapperProcedureHelper>(),
        provider.GetRequiredService<DapperContextAccessor>()
    )
);
```

---

## 📊 Best Practices

### 1. Idempotency Key Format
```
Format: {request_type}-{entity_id}-{user_id}-{timestamp}-{random}

Examples:
- hold-evt5-user1-20260405100000-abc123
- payment-order123-user1-20260405100000-def456
- cancel-hold456-user1-20260405100000-ghi789
```

### 2. Request Hash Computation
```csharp
// SHA256 hash của request JSON
using SHA256 = System.Security.Cryptography.SHA256.Create()
var json = JsonSerializer.Serialize(request);
var hash = SHA256.ComputeHash(Encoding.UTF8.GetBytes(json));
var hashString = Convert.ToBase64String(hash);
```

### 3. Expiration Strategy
```
Business Type          TTL
─────────────────────────────
hold_seats            10 minutes
payment              1 hour
cancel_hold          5 minutes
refund               1 hour
```

### 4. Response Snapshot Storage
```csharp
// Chỉ store những thông tin cần thiết
var snapshot = new {
    hold_id = result.hold_id,
    hold_code = result.hold_code,
    hold_expires_at = result.hold_expires_at,
    held_seats = result.held_seats
};

var json = JsonSerializer.Serialize(snapshot);
// Store vào response_snapshot field
```

---

## 🔧 Background Job: Clean Expired Records

```csharp
// Chạy mỗi 1 giờ để xóa các bản ghi hết hạn
public class CleanExpiredIdempotencyJob
{
    private readonly IIdempotencyRequestRepository _repository;
    
    public async Task Execute()
    {
        // Xóa tất cả bản ghi có expired_at < now()
        var expiredRecords = await GetExpiredRecords();
        
        foreach (var record in expiredRecords)
        {
            await _repository.DeleteAsync(
                new IdempotencyRequestDeleteDto { 
                    idempotency_id = record.idempotency_id 
                });
        }
    }
}
```

---

## 📈 Performance Considerations

### Database Indexes (Recommended)
```sql
-- Quan trọng nhất: Giúp lookup nhanh
CREATE INDEX idx_idempotency_key_type 
ON ticketing.idempotency_request(idempotency_key, request_type);

-- Hỗ trợ cleanup job
CREATE INDEX idx_idempotency_expired_at 
ON ticketing.idempotency_request(expired_at);

-- Hỗ trợ user queries
CREATE INDEX idx_idempotency_user_id 
ON ticketing.idempotency_request(user_id, created_at);
```

### Query Performance
```
GetByKeyAndType:     ~2-5ms    (Index lookup)
GetPagedList:        ~10-20ms  (Pagination)
GetByUserId:         ~5-15ms   (User query)
```

---

## ❌ Common Mistakes to Avoid

### 1. Not Computing Request Hash
```csharp
// ❌ WRONG: Cùng idempotency_key nhưng payload khác
var key = "hold-evt5-user1-001";  // Same key
// Backend không detect payload đã thay đổi

// ✅ RIGHT: Include request hash
var hash = ComputeHash(request);
// Backend detect request payload khác nhau
```

### 2. Not Storing Response Snapshot
```csharp
// ❌ WRONG: Không store response
idempotency.response_snapshot = null;

// ✅ RIGHT: Store response
idempotency.response_snapshot = JsonSerializer.Serialize(result.Data);
```

### 3. Not Setting Expiration
```csharp
// ❌ WRONG: Không set TTL
idempotency.expired_at = DateTime.MaxValue;

// ✅ RIGHT: Set reasonable TTL
idempotency.expired_at = DateTime.UtcNow.AddMinutes(10);
```

### 4. Not Handling Concurrent Requests
```csharp
// ❌ WRONG: Race condition nếu 2 request cùng lúc
if (existingRequest == null)
{
    // Insert new
}

// ✅ RIGHT: Use database constraint + unique index
// Database handles concurrency automatically
```

---

## 🎯 Integration Checklist

- [ ] Execute SQL functions in PostgreSQL
- [ ] Add DI registration for IIdempotencyRequestRepository
- [ ] Implement idempotency check in HoldSeat UseCase
- [ ] Implement idempotency check in Payment UseCase
- [ ] Add request hash computation helper
- [ ] Create response snapshot serialization helper
- [ ] Create database indexes for performance
- [ ] Create cleanup background job
- [ ] Add unit tests for idempotency logic
- [ ] Add integration tests for duplicate request handling

---

## 📚 Related Documentation

- SeatHold Module: `SEATHOLD_MODULE_IMPLEMENTATION.md`
- Payment Module: `PAYMENT_MODULE_IMPLEMENTATION.md`
- Complete Summary: `COMPLETE_MODULES_SUMMARY.md`

---

**Status**: ✅ **Infrastructure Complete**  
**Files Created**: 3 (Entity + DTOs + Repository)  
**SQL Functions**: 8 (6 core + 2 custom)  
**Ready for**: UseCase implementation with idempotency checks  

