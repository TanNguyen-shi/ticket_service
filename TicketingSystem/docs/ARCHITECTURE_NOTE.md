# Ghi chú Kiến trúc TicketingSystem (Onboarding)

Tài liệu này mô tả luồng xử lý **hiện tại (as-is)** để thành viên mới có thể lần theo 1 request end-to-end nhanh nhất.

## 1) Cấu trúc solution

- `TicketingSystem/` (Web API): điểm vào HTTP, middleware, cấu hình auth, Swagger.
- `Ticketing.Application/` (Use cases): điều phối luồng nghiệp vụ ở mức ứng dụng (ví dụ venue use cases).
- `Ticketing.Domain/` (Domain services): logic nghiệp vụ + ranh giới transaction.
- `Ticketing.Infrastructure/` (Hạ tầng): truy cập Dapper/PostgreSQL, JWT token service, DTO/helper dùng chung.

Composition root chính nằm ở `TicketingSystem/Program.cs`.

## 2) Luồng gọi API chuẩn (Venue)

Với một endpoint cần auth như insert venue:

1. Controller nhận HTTP request tại `TicketingSystem/Controllers/Venue/VenueController.cs`.
2. Controller gọi use case `IVenueUseCases` (`Ticketing.Application/UseCases/Venue/VenueUseCases.cs`).
3. Use case map request DTO -> domain entity rồi gọi `IVenueDomainService` (`Ticketing.Domain/Domain/Venue/VenueDomainService.cs`).
4. Domain service mở transaction qua `IVenueUnitOfWork` và gọi repository.
5. Repository (`Ticketing.Infrastructure/Repositories/Repository.cs` + `VenueRepository.cs`) tạo tên DB function rồi chuyển cho Dapper helper.
6. Dapper helper thực thi PostgreSQL function và trả kết quả scalar/list.
7. Kết quả được bọc trong `ResponseMessage<T>` và trả ngược lại qua các layer.

## 3) Luồng Auth/JWT

- `Jwt` options được bind trong `TicketingSystem/Program.cs` từ config section `Jwt`.
- Việc tạo token thực hiện tại `Ticketing.Infrastructure/JWT/JwtTokenService.cs`.
- Token chứa các claim: `nameidentifier`, `name`, `full_name`, `user_type`, và role claims.
- Lấy thông tin user hiện tại qua `IUserHelper` (`Ticketing.Infrastructure/Helpers/Impl/UserHelper.cs`) dùng `IHttpContextAccessor`.
- Endpoint cần bảo vệ dùng `[Authorize]` (ví dụ `TicketingSystem/Controllers/Venue/VenueController.cs`).

## 4) Kiểu truy cập Database (quan trọng)

Hạ tầng hiện tại đang theo hướng gọi **PostgreSQL FUNCTION** (không phải PROCEDURE):

- Generic repository resolve tên function theo mẫu `schema.table_action` trong `Ticketing.Infrastructure/Repositories/Repository.cs`.
  - Ví dụ: `ticketing.venue_insert`, `ticketing.venue_getbyid`, `ticketing.venue_getpagedlist`.
- `Ticketing.Infrastructure/Helpers/Impl/DapperProcedureHelper.cs` build SQL theo dạng:
  - `select schema.func(p_x => @p_x, ...)` cho các luồng scalar/non-query.
  - Với cursor: gọi function trước, sau đó `FETCH ALL IN "cursor_name"`.
- Vòng đời transaction được quản lý tại `Ticketing.Infrastructure/Repositories/UnitOfWork.cs`.

## 5) Các điểm cấu hình/runtime

- Startup API và thứ tự middleware: `TicketingSystem/Program.cs`.
- Development profile và URL: `TicketingSystem/Properties/launchSettings.json`.
- Cấu hình local cho JWT + DB: `TicketingSystem/appsettings.Development.json`.
- Cấu hình app mặc định: `TicketingSystem/appsettings.json`.

## 6) Checklist onboarding nhanh

- Đọc `TicketingSystem/Program.cs` trước (DI, auth, thứ tự middleware).
- Trace 1 feature end-to-end bằng nhóm file Venue (`Controller -> UseCases -> Domain -> Repository`).
- Xác nhận loại object DB trước khi viết SQL (`FUNCTION` là kiểu được helper hiện tại kỳ vọng).
- Khi thêm API cần auth, dùng `[Authorize]` và lấy user context qua `IUserHelper`.
- Giữ format response thống nhất với `ResponseMessage<T>`.

---
Nếu sau này team chuẩn hóa theo Clean Architecture chặt hơn, tài liệu này cần cập nhật lại (đặc biệt ở auth path và vị trí DTO dùng chéo layer).
