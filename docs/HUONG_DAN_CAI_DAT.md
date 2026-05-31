# Hướng Dẫn Cài Đặt và Chạy Hệ Thống

**Đề tài**: Xây dựng hệ thống đặt vé sự kiện trực tuyến với khả năng chịu tải đồng thời cao  
**Sinh viên**: Nguyễn Thiện Tân — 25410129

---

## 1. Yêu Cầu Môi Trường

| Công cụ | Phiên bản | Tải về |
|---------|-----------|--------|
| .NET SDK | 8.0.x | https://dotnet.microsoft.com/download/dotnet/8.0 |
| Docker Desktop | 4.x trở lên | https://www.docker.com/products/docker-desktop |
| Git | Bất kỳ | https://git-scm.com |
| pgAdmin 4 *(tùy chọn)* | 7.x trở lên | https://www.pgadmin.org |

> **Lưu ý**: Docker Desktop phải đang chạy trước khi thực hiện các bước bên dưới.

---

## 2. Lấy Source Code

```bash
git clone <repository-url>
cd TicketingSystem
```

Hoặc giải nén từ file ZIP được cung cấp:

```
Nguyễn Thiện Tân - 25410129/
└── Source/
    └── TicketingSystem/   ← làm việc trong thư mục này
```

---

## 3. Khởi Động PostgreSQL và Redis bằng Docker

Dự án sử dụng Docker Compose để chạy PostgreSQL 16 và Redis 7.

### 3.1 Tạo file `.env`

Tạo file `.env` tại thư mục `TicketingSystem/Docker/` (cùng cấp với `docker-compose.yml`):

```env
POSTGRES_DB=ticketing_db
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres123
```

### 3.2 Khởi động containers

```bash
cd TicketingSystem/Docker
docker-compose up -d
```

Kiểm tra containers đang chạy:

```bash
docker ps
```

Kết quả mong đợi:

```
CONTAINER ID   IMAGE         NAMES               STATUS
xxxxxxxxxxxx   postgres:16   ticketing-postgres  Up (healthy)
xxxxxxxxxxxx   redis:7       ticketing-redis     Up
```

> **Lưu ý**: PostgreSQL cần khoảng 10–15 giây để khởi động lần đầu. Chờ status chuyển sang `healthy` trước khi tiếp tục.

---

## 4. Khởi Tạo Database

### 4.1 Tạo schema và bảng

Kết nối vào PostgreSQL và chạy file tạo schema. Bạn có thể dùng pgAdmin hoặc lệnh `psql`:

```bash
# Kết nối bằng psql (chạy trong Docker container)
docker exec -it ticketing-postgres psql -U postgres -d ticketing_db
```

Sau đó chạy file schema (nếu có trong thư mục `database/schema/`).

### 4.2 Chạy các Stored Functions

Chạy lần lượt các file SQL trong thư mục `database/functions/` theo thứ tự sau:

```bash
# Từ thư mục gốc TicketingSystem/
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
```

> **Hoặc** dùng pgAdmin: kết nối tới `localhost:5432`, database `ticketing_db`, mở từng file `.sql` và chạy (F5).

---

## 5. Cấu Hình Backend

File cấu hình chính: `TicketingSystem/appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ticketing_db;Username=postgres;Password=postgres123"
  },
  "Redis": {
    "ConnectionString": "localhost:6379,abortConnect=false",
    "InstanceName": "ticketing:dev:",
    "DefaultTtlSeconds": 300
  },
  "Jwt": {
    "Issuer": "TicketingSystem",
    "Audience": "TicketingSystem.Client",
    "SecretKey": "8cc9711c75589e92b505a0b78345f4ca",
    "ExpireMinutes": 120
  }
}
```

> Nếu bạn đổi mật khẩu PostgreSQL ở bước 3.1, cập nhật `Password=` tương ứng trong file này.

---

## 6. Chạy Backend API

```bash
# Từ thư mục gốc TicketingSystem/
dotnet run --project TicketingSystem/TicketingSystem.csproj --environment Development
```

Hoặc dùng IDE (Visual Studio / Rider): mở file `TicketingSystem.sln`, chọn profile `Development` và nhấn Run.

**API sẵn sàng tại:**
- API: `http://localhost:5000`
- Swagger UI: `http://localhost:5000/swagger`

Kết quả log mong đợi:

```
[INF] Now listening on: http://localhost:5000
[INF] Application started. Press Ctrl+C to shut down.
```

---

## 7. Chạy Frontend (nếu có)

> Frontend là project React riêng biệt. Tham khảo hướng dẫn cài đặt trong thư mục `Source/Frontend/`.

```bash
cd Source/Frontend
npm install
npm run dev
```

Frontend chạy tại: `http://localhost:3000`

---

## 8. Kiểm Tra Hệ Thống

### 8.1 Kiểm tra API hoạt động

Mở trình duyệt, truy cập: `http://localhost:5000/swagger`

Thử endpoint đăng nhập admin:

```
POST /api/auth/login
Body: { "username": "admin", "password": "admin123" }
```

### 8.2 Kiểm tra kết nối Redis

```bash
docker exec -it ticketing-redis redis-cli ping
# Kết quả: PONG
```

### 8.3 Kiểm tra kết nối PostgreSQL

```bash
docker exec -it ticketing-postgres psql -U postgres -d ticketing_db -c "\dt ticketing.*"
```

---

## 9. Dừng Hệ Thống

```bash
# Dừng API: Ctrl+C trong terminal đang chạy dotnet

# Dừng Docker containers
cd TicketingSystem/Docker
docker-compose down

# Dừng và xóa toàn bộ data (reset hoàn toàn)
docker-compose down -v
```

---

## 10. Cấu Trúc Thư Mục Source

```
TicketingSystem/
├── TicketingSystem.sln               # Solution file
├── global.json                       # .NET SDK version lock (8.0.x)
├── TicketingSystem/                  # Web API (entry point)
│   ├── Program.cs
│   ├── Controllers/
│   │   ├── Admin/                    # Admin endpoints
│   │   └── Client/                   # Client endpoints
│   ├── BackgroundServices/           # SeatHoldExpiry job
│   ├── Docker/
│   │   ├── docker-compose.yml        # PostgreSQL + Redis
│   │   └── Dockerfile
│   ├── appsettings.json
│   └── appsettings.Development.json
├── Ticketing.Application/            # Use Cases layer
├── Ticketing.Domain/                 # Domain Services layer
├── Ticketing.Infrastructure/         # Repositories, Dapper, JWT, Redis
└── database/
    └── functions/                    # PostgreSQL stored functions
```

---

## 11. Xử Lý Lỗi Thường Gặp

| Lỗi | Nguyên nhân | Cách xử lý |
|-----|-------------|------------|
| `Connection refused (port 5432)` | PostgreSQL chưa chạy | Chạy `docker-compose up -d` và chờ `healthy` |
| `Connection refused (port 6379)` | Redis chưa chạy | Kiểm tra container Redis với `docker ps` |
| `function ticketing.xxx does not exist` | Chưa chạy file SQL | Chạy lại bước 4.2 |
| `Jwt:SecretKey must be at least 16 bytes` | Thiếu cấu hình JWT | Kiểm tra `appsettings.Development.json` |
| Port 5000 đã bị dùng | Có process khác | Đổi port trong `launchSettings.json` |

---

## 12. Thông Tin Liên Hệ

**Sinh viên thực hiện**: Nguyễn Thiện Tân — 25410129  
**Cán bộ hướng dẫn**: ThS. Mai Xuân Hùng — `hungmx@uit.edu.vn`
