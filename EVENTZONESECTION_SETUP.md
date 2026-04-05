# EventZoneSection Integration - Setup Guide

## 📋 Tóm tắt thay đổi

Đã tích hợp **EventZoneSection** vào project theo cấu trúc Clean Architecture hiện tại.

### ✅ Files tạo mới

**Infrastructure Layer:**
1. `Entities/EventZoneSection/Response/EventZoneSectionEntity.cs` - Entity mapping
2. `DTOs/Admin/EventZoneSection/Request/` (5 files)
   - `EventZoneSectionCreateRequest.cs`
   - `EventZoneSectionUpdateRequest.cs`
   - `EventZoneSectionDeleteRequest.cs`
   - `EventZoneSectionGetByIdRequest.cs`
   - `EventZoneSectionGetPagedListRequest.cs`
3. `DTOs/Admin/EventZoneSection/Response/` (2 files)
   - `EventZoneSectionDto.cs`
   - `EventZoneSectionListDto.cs`
4. `Repositories/EventZoneSection/EventZoneSectionRepository.cs` - Repository + Interface

**Database:**
- `database/functions/event_zone_section_functions.sql` - 6 SQL Functions

### 🔄 Files cập nhật

1. `Repositories/Event/EventUnitOfWork.cs`
   - Thêm `IEventZoneSectionRepository` property
   - Cập nhật constructor

2. `Configurations/ConfigDI/InfrastructureConfigDI.cs`
   - Thêm import `EventZoneSection`
   - Đăng ký DI repository

---

## 🗄️ SQL Functions cần chạy trên PostgreSQL

Chạy file: `database/functions/event_zone_section_functions.sql`

**6 Functions tạo:**
1. `event_zone_section_insert` - Thêm mới
2. `event_zone_section_update` - Cập nhật
3. `event_zone_section_delete` - Xóa (soft delete)
4. `event_zone_section_getbyid` - Lấy chi tiết
5. `event_zone_section_getbyeventzoneid` - Lấy theo zone
6. `event_zone_section_getbyeventid` - Lấy theo event
7. `event_zone_section_getpagedlist` - Lấy danh sách phân trang

---

## 🚀 Cách sử dụng

### Trong Domain Service:

```csharp
public class YourDomainService
{
    private readonly IEventUnitOfWork _unitOfWork;

    // Lấy sections theo event zone
    var sections = await _unitOfWork.EventZoneSection
        .GetByEventZoneId<EventZoneSectionDto>(
            new { event_zone_id = 1 }, 
            cancellationToken);

    // Lấy sections theo event
    var eventSections = await _unitOfWork.EventZoneSection
        .GetByEventId<EventZoneSectionDto>(
            new { event_id = 1 }, 
            cancellationToken);
}
```

---

## 📝 Liên kết

- ✅ EventZoneSection repository đã được inject vào `IEventUnitOfWork`
- ✅ DTOs hoàn toàn theo convention hiện tại
- ✅ SQL Functions dùng refcursor giống các module khác
- ✅ Sẵn sàng tạo Domain Service + UseCase + Controller

---

## 🎯 Bước tiếp theo

1. **Chạy SQL Functions** trên DB
2. **Tạo EventZoneSectionDomainService** để handle business logic
3. **Tạo UseCase** cho các thao tác CRUD
4. **Tạo Controller** endpoint `/api/admin/event-zone-section`

Đã sẵn sàng! 🎉

