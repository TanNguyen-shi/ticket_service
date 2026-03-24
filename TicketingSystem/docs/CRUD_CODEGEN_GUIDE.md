# Hướng dẫn sinh CRUD theo chuẩn project

Tài liệu này dùng làm "playbook" để sinh code CRUD theo đúng flow hiện tại của dự án:

`Controller -> UseCase -> Domain -> Repository -> PostgreSQL Function`

Mục tiêu: khi bạn gửi **table + function SQL**, có thể tích hợp nhanh, đúng thư mục, đúng naming rule, đúng pattern đang dùng.

---

## 1) Input bắt buộc khi yêu cầu sinh CRUD

Khi gửi yêu cầu, cung cấp tối thiểu:

- Tên module (ví dụ: `Event`)
- Tên table (ví dụ: `ticketing.event`)
- SQL function cho các action:
  - `schema.table_insert`
  - `schema.table_update`
  - `schema.table_delete`
  - `schema.table_getbyid`
  - `schema.table_getpagedlist`
- Danh sách cột chính của table + kiểu dữ liệu
- Quy ước trả về của function:
  - insert: trả ID (`bigint`/`int`)
  - update/delete: trả `boolean` hoặc `int`
  - getbyid/getpagedlist: trả `refcursor` (theo pattern hiện tại)

Khuyến nghị gửi thêm:

- Ràng buộc unique/check
- Các field audit (`created_by`, `updated_by`, `deleted_by`)
- Ví dụ request/response mong muốn

---

## 2) Naming rule chuẩn (theo codebase hiện tại)

### 2.1 Tên module

- Module dùng PascalCase số ít: `Venue`, `Event`, `Ticket`
- Folder module cũng dùng PascalCase

### 2.2 Tên file và class

- Controller: `{Module}Controller`
- UseCase interface: `I{Module}UseCases`
- UseCase class: `{Module}UseCases`
- Domain interface: `I{Module}DomainService`
- Domain class: `{Module}DomainService`
- Repository interface: `I{Module}Repository`
- Repository class: `{Module}Repository`
- UnitOfWork interface: `I{Module}UnitOfWork`
- UnitOfWork class: `{Module}UnitOfWork`
- Entity: `{Module}Entity`

### 2.3 Tên field

- DTO/Entity trong project đang dùng `snake_case` để map DB trực tiếp.
- Parameter SQL dùng prefix `p_` (helper hiện tại tự chuẩn hóa theo rule này).

---

## 3) Cấu trúc thư mục cần tạo/sửa

Với module `Event`, tạo/sửa theo cấu trúc:

- `TicketingSystem/Controllers/Event/EventController.cs`
- `Ticketing.Application/UseCases/Event/Interfaces/IEventUseCases.cs`
- `Ticketing.Application/UseCases/Event/EventUseCases.cs`
- `Ticketing.Domain/Domain/Event/Interfaces/IEventDomainService.cs`
- `Ticketing.Domain/Domain/Event/EventDomainService.cs`
- `Ticketing.Infrastructure/Repositories/Event/EventRepository.cs`
- `Ticketing.Infrastructure/Repositories/Event/EventUnitOfWork.cs`
- `Ticketing.Infrastructure/Entities/Event/EventEntity.cs`
- `Ticketing.Infrastructure/Entities/Event/Request/*.cs` (nếu cần)
- `Ticketing.Infrastructure/Entities/Event/Response/*.cs` (nếu cần)
- `Ticketing.Infrastructure/DTOs/Event/Request/*.cs`
- `Ticketing.Infrastructure/DTOs/Event/Response/*.cs`

Cập nhật DI:

- `Ticketing.Application/ConfigDI/UseCaseConfigureDI.cs`
- `Ticketing.Domain/ConfigDI/DomainConfigDI.cs`
- `Ticketing.Infrastructure/Configurations/ConfigDI/InfrastructureConfigDI.cs`

---

## 4) Chuẩn flow code cho từng layer

### 4.1 Controller

- Dùng `[ApiController]`, `[Authorize]`, route chuẩn: `api/admin/{resource}`
- Inject `I{Module}UseCases` + `IUserHelper`
- Nhận DTO từ request (`[FromBody]`/`[FromQuery]`)
- Truyền `user.UserId` xuống use case cho các action ghi dữ liệu
- Trả `ResponseMessage<T>` đồng nhất

Action chuẩn:

- `insert`
- `update`
- `delete`
- `getbyid`
- `getpagedlist`

### 4.2 UseCase

- Map DTO -> `Entity`
- Gọi `I{Module}DomainService`
- Bọc kết quả về `ResponseMessage<T>`
- Không xử lý SQL trực tiếp

### 4.3 Domain

- Mở transaction qua `I{Module}UnitOfWork` cho insert/update/delete
- Gọi repository với object param theo function signature
- Commit/Rollback theo kết quả
- Query (`getbyid/getpagedlist`) không cần transaction nếu không cần thiết

### 4.4 Repository

- Kế thừa `Repository<{Module}Entity>`
- Override:
  - `Schema => "ticketing"` (hoặc schema được cung cấp)
  - `TableName => "event"` (snake_case)
- Tận dụng generic methods hiện có:
  - `InsertAsync`, `UpdateAsync`, `DeleteAsync`
  - `GetAsync`, `GetPagedAsync`

---

## 5) Quy ước PostgreSQL function (bắt buộc khớp code)

Pattern tên hàm phải khớp `schema.table_action`, ví dụ:

- `ticketing.event_insert`
- `ticketing.event_update`
- `ticketing.event_delete`
- `ticketing.event_getbyid`
- `ticketing.event_getpagedlist`

Lưu ý quan trọng:

- Hạ tầng hiện tại gọi theo **FUNCTION** (không phải PROCEDURE).
- Nếu DB tạo PROCEDURE sẽ gây lỗi dạng `is not a procedure`/mismatch khi gọi.
- Với query list/detail theo pattern hiện tại, function nên trả cursor name để helper `FETCH ALL`.

---

## 6) Mẫu yêu cầu để AI sinh code đúng ngay lần đầu

Dùng mẫu này khi gửi yêu cầu:

```md
Sinh CRUD cho module Event theo chuẩn project hiện tại.

Thong tin DB:
- Table: ticketing.event
- Functions:
  - ticketing.event_insert(...)
  - ticketing.event_update(...)
  - ticketing.event_delete(...)
  - ticketing.event_getbyid(...)
  - ticketing.event_getpagedlist(...)

Yeu cau:
- Sinh day du Controller -> UseCase -> Domain -> Repository
- Tao DTO Request/Response va Entity dung folder convention
- Cap nhat DI o 3 file ConfigDI
- Route theo chuan api/admin/event
- Dung ResponseMessage<T> giong module Venue
```

---

## 7) Definition of Done (DoD)

Một module CRUD được xem là hoàn tất khi:

- Build solution không lỗi
- Có đủ 5 endpoint CRUD chuẩn
- DI đã đăng ký đủ ở Application/Domain/Infrastructure
- Endpoint bảo vệ bằng JWT hoạt động với `[Authorize]`
- Insert/Update/Delete chạy transaction đúng
- GetById/GetPagedList trả dữ liệu đúng schema response

---

## 8) Checklist thực thi nhanh cho mỗi module mới

- [ ] Có đủ SQL function đúng naming
- [ ] Tạo DTO + Entity đúng thư mục
- [ ] Tạo UseCase interface/class
- [ ] Tạo Domain interface/class
- [ ] Tạo Repository + UnitOfWork
- [ ] Tạo Controller + route + authorize
- [ ] Cập nhật 3 file DI
- [ ] Build và test nhanh 5 endpoint trên Swagger

