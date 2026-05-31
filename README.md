# Ticketing System — Backend API

Hệ thống đặt vé sự kiện trực tuyến với khả năng chịu tải đồng thời cao.  
Xây dựng bằng **.NET 8**, **PostgreSQL**, **Redis**, theo kiến trúc **Clean Architecture**.

**Sinh viên**: Nguyễn Thiện Tân — 25410129  
**CBHD**: ThS. Mai Xuân Hùng

---

## Yêu Cầu

| Công cụ | Phiên bản |
|---------|-----------|
| .NET SDK | 8.0.x |
| Docker Desktop | 4.x+ |
| Git | Bất kỳ |

---

## Cài Đặt Nhanh

### 1. Clone repo

```bash
git clone <repository-url>
cd TicketingSystem
```

### 2. Khởi động PostgreSQL + Redis

```bash
cd TicketingSystem/Docker

# Tạo file .env
echo "POSTGRES_DB=ticketing_db
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres123" > .env

# Chạy containers
docker-compose up -d
```

Chờ PostgreSQL `healthy` (khoảng 15 giây):

```bash
docker ps   # STATUS phải là "Up (healthy)"
```

### 3. Khởi tạo database

```bash
# Từ thư mục gốc TicketingSystem/

# Schema (tables, indexes, constraints)
docker exec -i ticketing-postgres psql -U postgres -d ticketing_db < database/ticketing_schema.sql

# Stored functions
docker exec -i ticketing-postgres psql -U postgres -d ticketing_db < database/functions/customer_functions.sql
docker exec -i ticketing-postgres psql -U postgres -d ticketing_db < database/functions/event_zone_section_functions.sql
docker exec -i ticketing-postgres psql -U postgres -d ticketing_db < database/functions/event_seat_inventory_functions.sql
docker exec -i ticketing-postgres psql -U postgres -d ticketing_db < database/functions/event_publish_log_functions.sql
docker exec -i ticketing-postgres psql -U postgres -d ticketing_db < database/functions/idempotency_functions.sql
docker exec -i ticketing-postgres psql -U postgres -d ticketing_db < database/functions/seat_hold_functions.sql
docker exec -i ticketing-postgres psql -U postgres -d ticketing_db < database/functions/checkout_functions.sql
docker exec -i ticketing-postgres psql -U postgres -d ticketing_db < database/functions/payment_functions.sql
docker exec -i ticketing-postgres psql -U postgres -d ticketing_db < database/functions/ticket_functions.sql
docker exec -i ticketing-postgres psql -U postgres -d ticketing_db < database/functions/customer_seat_hold_migration.sql

# Seed data (venue, event, seat, admin account, ...)
docker exec -i ticketing-postgres psql -U postgres -d ticketing_db < database/ticketing_seed.sql
```

### 4. Chạy API

```bash
# Từ thư mục gốc TicketingSystem/
dotnet run --project TicketingSystem/TicketingSystem.csproj --environment Development
```

**API**: `http://localhost:5000`  
**Swagger**: `http://localhost:5000/swagger`

---

## Tài Khoản Demo

| Role | Cách lấy |
|------|----------|
| Admin | Xem `database/ticketing_seed.sql` → bảng `sys_user` |
| Customer | Đăng ký qua `POST /api/client/auth/register` |

---

## Kiến Trúc

```
Request → Controller
    ↓
UseCase (Application layer)
    ↓
Domain Service (Business logic + Transaction)
    ↓
Repository (Dapper + Stored Procedures)
    ↓
PostgreSQL
```

| Layer | Project | Vai trò |
|-------|---------|---------|
| Presentation | `TicketingSystem/` | Controllers, Middleware, JWT |
| Application | `Ticketing.Application/` | Use Cases, DTO mapping |
| Domain | `Ticketing.Domain/` | Business rules, UnitOfWork |
| Infrastructure | `Ticketing.Infrastructure/` | Repository, Dapper, Redis, JWT |

---

## API Endpoints

### Admin (yêu cầu JWT)

| Module | Route |
|--------|-------|
| Auth | `POST /api/auth/login` |
| Event | `GET/POST/PUT/DELETE /api/admin/event` |
| EventZone | `GET/POST/PUT/DELETE /api/admin/event-zone` |
| EventZonePrice | `GET/POST/PUT/DELETE /api/admin/event-zone-price` |
| Venue | `GET/POST/PUT/DELETE /api/admin/venue` |
| VenueSection | `GET/POST/PUT/DELETE /api/admin/venue-section` |
| VenueSeat | `GET/POST/PUT/DELETE /api/admin/venue-seat` |
| SysUser | `GET/POST/PUT/DELETE /api/admin/sys-user` |
| SysRole | `GET/POST/PUT/DELETE /api/admin/sys-role` |
| SysUserRole | `GET/POST/PUT/DELETE /api/admin/sys-user-role` |

### Client (public + JWT)

| Module | Route |
|--------|-------|
| Auth | `POST /api/client/auth/register` |
| | `POST /api/client/auth/login` |
| Event | `GET /api/client/event/featured` |
| | `GET /api/client/event/trending` |
| | `GET /api/client/event/upcoming` |
| | `GET /api/client/event/search` |
| | `GET /api/client/event/detail` ← trả về seat map + trạng thái ghế |
| Booking | `POST /api/client/booking/hold-seat` |
| | `POST /api/client/booking/checkout` |
| | `DELETE /api/client/booking/release/{holdId}` |
| Ticket | `GET /api/client/ticket/my-tickets` |
| | `GET /api/client/ticket/detail` |

---

## Tính Năng Chính

- **Giữ chỗ 10 phút** — Redis lock, background job tự động trả ghế sau 60s
- **Chống overselling** — Optimistic locking (`version`) + Redis seat-level lock
- **Idempotency** — Ngăn double-click / retry tạo hold trùng
- **Seat map real-time** — `detail` trả về trạng thái ghế theo từng request
- **JWT Auth** — Admin và Customer dùng token riêng biệt
- **Soft delete** — Mọi entity có `is_deleted`, không xóa vật lý

---

## Cấu Trúc Thư Mục

```
TicketingSystem/
├── TicketingSystem.sln
├── global.json                          # .NET SDK 8.0.x
├── TicketingSystem/                     # Web API (entry point)
│   ├── Controllers/Admin/
│   ├── Controllers/Client/
│   ├── BackgroundServices/              # SeatHoldExpiry
│   ├── Docker/
│   │   ├── docker-compose.yml           # PostgreSQL 16 + Redis 7
│   │   └── Dockerfile
│   ├── appsettings.json
│   └── appsettings.Development.json
├── Ticketing.Application/               # Use Cases
├── Ticketing.Domain/                    # Domain Services
├── Ticketing.Infrastructure/            # Repositories, Dapper, JWT, Redis
├── database/
│   ├── ticketing_schema.sql             # DDL toàn bộ schema
│   ├── ticketing_seed.sql               # Seed data demo
│   └── functions/                       # Stored procedures PostgreSQL
└── docs/
    ├── HUONG_DAN_CAI_DAT.md
    ├── DATABASE.md
    └── ARCHITECTURE.md
```

---

## Công Nghệ

| Công nghệ | Phiên bản | Mục đích |
|-----------|-----------|---------|
| ASP.NET Core | 8.0 | Web API |
| PostgreSQL | 16 | Database chính |
| Redis | 7 | Cache + distributed lock |
| Dapper | 2.1.72 | Stored Procedure ORM |
| Npgsql | 10.0.2 | PostgreSQL driver |
| JWT Bearer | 8.0.25 | Authentication |
| Serilog | 10.0.0 | Structured logging |
| Swagger | 6.6.2 | API docs |

---

## Xử Lý Lỗi Thường Gặp

| Lỗi | Cách xử lý |
|-----|------------|
| `Connection refused :5432` | `docker-compose up -d`, chờ status `healthy` |
| `Connection refused :6379` | Kiểm tra container Redis |
| `function ticketing.xxx does not exist` | Chạy lại files trong `database/functions/` |
| Port 5000 bị chiếm | Đổi port trong `launchSettings.json` |

---

## Tài Liệu

- [`docs/HUONG_DAN_CAI_DAT.md`](docs/HUONG_DAN_CAI_DAT.md) — Cài đặt đầy đủ từng bước
- [`docs/DATABASE.md`](docs/DATABASE.md) — Schema, quan hệ bảng, indexes
- [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) — Luồng xử lý chi tiết
- [`docs/API.md`](docs/API.md) — Tài liệu API đầy đủ
