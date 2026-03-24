# TicketingSystem - Backend Ticket Service

Backend service cho do an tot nghiep, cung cap API quan ly he thong ban ve su kien (event ticketing).

## Muc tieu du an

- Xay dung API theo kien truc tach lop ro rang.
- Ho tro luong CRUD cho cac module nghiep vu ve su kien, dia diem, don hang va ve.
- Dong nhat response API va co san co che xac thuc JWT.

## Kien truc tong quan

Du an su dung flow:

`Controller -> UseCase -> Domain -> Repository -> PostgreSQL Function`

Cac layer chinh:

- `TicketingSystem/`: Web API (Program, Controller, Middleware, Swagger, Auth).
- `Ticketing.Application/`: Application use case (dieu phoi luong xu ly).
- `Ticketing.Domain/`: Domain service (xu ly nghiep vu, transaction boundary).
- `Ticketing.Infrastructure/`: Repository, UnitOfWork, Dapper helper, JWT service, DTO/Entity.

## Cau truc solution

- `TicketingSystem.sln`: Solution chinh.
- `global.json`: Khoa SDK .NET (`8.0.0`, roll-forward minor).
- `TicketingSystem/appsettings.Development.json`: Cau hinh local (PostgreSQL + JWT).
- `TicketingSystem/docs/ARCHITECTURE_NOTE.md`: Ghi chu onboarding kien truc.
- `TicketingSystem/docs/CRUD_CODEGEN_GUIDE.md`: Huong dan sinh CRUD theo convention project.

## Module nghiep vu hien co

Mot so module dang co trong source:

- `Auth`
- `Venue`, `VenueSection`, `VenueSeat`
- `Event`, `EventZone`, `EventSeatInventory`, `EventPublishLog`
- `TicketOrder`, `TicketOrderItem`, `Ticket`

## Cong nghe su dung

- `.NET 8` (ASP.NET Core Web API)
- `PostgreSQL`
- `Dapper`
- `JWT Bearer Authentication`
- `Serilog`
- `Swagger/OpenAPI`

## Dieu kien can

- .NET SDK 8.x
- PostgreSQL (local hoac container)
- (Tuy chon) Docker + Docker Compose

## Chay local

Tu thu muc goc repo:

```bash
cd "/Users/tanit/UIT/HK3/Project_Web_DoAnTN/TicketingSystem"
dotnet restore "TicketingSystem.sln"
dotnet build "TicketingSystem.sln"
dotnet run --project "TicketingSystem/TicketingSystem.csproj"
```

Mac dinh profile Development mo Swagger tai:

- `http://localhost:5025/swagger`
- hoac `https://localhost:7267/swagger`

## Chay bang Docker

File compose dang nam trong `TicketingSystem/Docker/docker-compose.yml`.

```bash
cd "/Users/tanit/UIT/HK3/Project_Web_DoAnTN/TicketingSystem/TicketingSystem/Docker"
docker compose --env-file .env up -d --build
```

Service API map cong:

- `http://localhost:5000`

## Auth va API docs

- Swagger duoc bat trong moi truong Development.
- API su dung JWT Bearer.
- Cau hinh JWT trong `TicketingSystem/appsettings.Development.json` (`Issuer`, `Audience`, `SecretKey`, `ExpireMinutes`).

## Quy uoc phat trien

- Dat ten module theo PascalCase, field DTO/Entity theo snake_case de map DB truc tiep.
- Luu y hieu nang voi pattern goi PostgreSQL FUNCTION theo convention `schema.table_action`.
- Tham khao them tai:
  - `TicketingSystem/docs/ARCHITECTURE_NOTE.md`
  - `TicketingSystem/docs/CRUD_CODEGEN_GUIDE.md`

## Gop y va mo rong

Neu mo rong module moi, uu tien giu dung convention hien tai:

- Tao du 5 action CRUD (`insert`, `update`, `delete`, `getbyid`, `getpagedlist`)
- Cap nhat DI tai 3 layer Application/Domain/Infrastructure
- Kiem tra lai Swagger + JWT + DB function mapping truoc khi merge
