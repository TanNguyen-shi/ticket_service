# API Endpoints Reference

---

## Authentication

### JWT Bearer Token

All protected endpoints require:

```
Authorization: Bearer <jwt_token>
```

Token obtained from `/api/auth/login`:
- Contains claims: `NameIdentifier` (user_id), `Name` (username), `full_name`, `user_type`, `role`
- TTL: Configurable in appsettings
- Validation: HS256 signature, issuer/audience checks

Token placement in request:
```http
POST /api/admin/event/insert HTTP/1.1
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
```

### Public Routes (No Auth Required)

- `POST /api/auth/login` — Login with credentials
- `GET /api/client/event/featured` — Browse featured events
- `GET /api/client/event/trending` — Browse trending events
- `GET /api/client/event/upcoming` — Browse upcoming events
- `GET /api/client/event/{id}` — Event detail (with seat inventory)

---

## Admin Endpoints

### Event Management

#### Create Event
```http
POST /api/admin/event/insert
Authorization: Bearer <token>
Content-Type: application/json

{
  "event_code": "EVT20260501",
  "event_name": "Sky Tour Live Concert - Hà Nội",
  "description": "Biggest concert of the year",
  "venue_id": 1,
  "banner_url": "https://example.com/banner.jpg",
  "start_time": "2026-05-15T19:00:00.000Z",
  "end_time": "2026-05-15T22:00:00.000Z",
  "sale_start_time": "2026-04-15T10:00:00.000Z",
  "sale_end_time": "2026-05-15T18:00:00.000Z",
  "status": "draft",
  "is_featured": true,
  "is_trending": false,
  "display_order": 1
}
```

**Response** (200 OK):
```json
{
  "issuccess": true,
  "status": "success",
  "message": "Thêm sự kiện thành công",
  "errorcode": 0,
  "data": 123,
  "errors": []
}
```

#### Update Event
```http
PUT /api/admin/event/update
Authorization: Bearer <token>
Content-Type: application/json

{
  "event_id": 123,
  "event_code": "EVT20260501",
  "event_name": "Sky Tour Live Concert - Updated",
  "description": "Updated description",
  "venue_id": 1,
  "banner_url": "https://example.com/banner.jpg",
  "start_time": "2026-05-15T19:00:00.000Z",
  "end_time": "2026-05-15T22:00:00.000Z",
  "sale_start_time": "2026-04-15T10:00:00.000Z",
  "sale_end_time": "2026-05-15T18:00:00.000Z",
  "status": "published",
  "published_at": "2026-04-01T00:00:00.000Z",
  "on_sale_at": "2026-04-15T10:00:00.000Z",
  "is_featured": true,
  "is_trending": true,
  "display_order": 2
}
```

**Response** (200 OK):
```json
{
  "issuccess": true,
  "status": "success",
  "message": "Cập nhật sự kiện thành công",
  "errorcode": 0,
  "data": 123,
  "errors": []
}
```

#### Delete Event (Soft Delete)
```http
DELETE /api/admin/event/delete
Authorization: Bearer <token>
Content-Type: application/json

{
  "event_id": 123
}
```

**Response** (200 OK):
```json
{
  "issuccess": true,
  "status": "success",
  "message": "Xóa sự kiện thành công",
  "errorcode": 0,
  "data": 123,
  "errors": []
}
```

#### Get Event Detail
```http
GET /api/admin/event/getbyid?event_id=123
Authorization: Bearer <token>
```

**Response** (200 OK):
```json
{
  "issuccess": true,
  "status": "success",
  "message": "Lấy chi tiết sự kiện thành công",
  "errorcode": 0,
  "data": {
    "event_id": 123,
    "event_code": "EVT20260501",
    "event_name": "Sky Tour Live Concert",
    "description": "...",
    "venue_id": 1,
    "venue_name": "Quốc Ops Stadium",
    "banner_url": "...",
    "start_time": "2026-05-15T19:00:00",
    "end_time": "2026-05-15T22:00:00",
    "sale_start_time": "2026-04-15T10:00:00",
    "sale_end_time": "2026-05-15T18:00:00",
    "status": "on_sale",
    "published_at": "2026-04-01T00:00:00",
    "on_sale_at": "2026-04-15T10:00:00",
    "is_featured": true,
    "is_trending": false,
    "display_order": 1,
    "created_by": 1,
    "created_by_name": "Admin User",
    "created_at": "2026-03-20T10:00:00",
    "updated_by": 1,
    "updated_by_name": "Admin User",
    "updated_at": "2026-04-05T15:30:00"
  },
  "errors": []
}
```

#### List Events (Paginated)
```http
GET /api/admin/event/getpagedlist?pagesize=20&offset=0&keysearch=sky&status=on_sale&venue_id=-1
Authorization: Bearer <token>
```

**Response** (200 OK):
```json
{
  "issuccess": true,
  "status": "success",
  "message": "Lấy danh sách sự kiện thành công",
  "errorcode": 0,
  "data": [
    {
      "row_index": 1,
      "row_total": 45,
      "event_id": 123,
      "event_code": "EVT20260501",
      "event_name": "Sky Tour Live Concert",
      "banner_url": "...",
      "start_time": "2026-05-15T19:00:00",
      "end_time": "2026-05-15T22:00:00",
      "status": "on_sale"
    }
  ],
  "errors": []
}
```

---

### Venue Management

#### Create Venue
```http
POST /api/admin/venue/insert
Authorization: Bearer <token>
Content-Type: application/json

{
  "venue_code": "VN001",
  "venue_name": "Quốc Ops Stadium",
  "address_line": "123 Nguyễn Huệ, Quận 1",
  "city": "Hà Nội",
  "country": "Vietnam",
  "capacity": 5000,
  "admin_email": "venue@example.com",
  "status": "active"
}
```

#### Update Venue
```http
PUT /api/admin/venue/update
Authorization: Bearer <token>
Content-Type: application/json

{
  "venue_id": 1,
  "venue_code": "VN001",
  "venue_name": "Quốc Ops Stadium - Updated",
  ...
}
```

#### Delete Venue
```http
DELETE /api/admin/venue/delete
Authorization: Bearer <token>
Content-Type: application/json

{
  "venue_id": 1
}
```

#### Get Venue Detail
```http
GET /api/admin/venue/getbyid?venue_id=1
Authorization: Bearer <token>
```

#### List Venues
```http
GET /api/admin/venue/getpagedlist?pagesize=20&offset=0&keysearch=quoc&status=active
Authorization: Bearer <token>
```

---

### Event Zone Management

#### Create Zone
```http
POST /api/admin/event-zone/insert
Authorization: Bearer <token>
Content-Type: application/json

{
  "event_id": 123,
  "zone_code": "FLOOR",
  "zone_name": "Sân sân",
  "base_price": 500000,
  "capacity": 1000,
  "status": "active"
}
```

#### Update Zone
```http
PUT /api/admin/event-zone/update
Authorization: Bearer <token>
Content-Type: application/json

{
  "event_zone_id": 1,
  "event_id": 123,
  "zone_code": "FLOOR",
  "zone_name": "Sân sân",
  "base_price": 550000,
  "capacity": 1000,
  "status": "active"
}
```

#### Delete Zone
```http
DELETE /api/admin/event-zone/delete
Authorization: Bearer <token>
Content-Type: application/json

{
  "event_zone_id": 1
}
```

#### Get Zone Detail
```http
GET /api/admin/event-zone/getbyid?event_zone_id=1
Authorization: Bearer <token>
```

#### List Zones
```http
GET /api/admin/event-zone/getpagedlist?pagesize=50&offset=0&event_id=123
Authorization: Bearer <token>
```

---

### Event Zone Pricing

#### Create Price Tier
```http
POST /api/admin/event-zone-price/insert
Authorization: Bearer <token>
Content-Type: application/json

{
  "event_zone_id": 1,
  "price": 500000,
  "currency": "VND",
  "start_time": "2026-04-15T10:00:00.000Z",
  "end_time": "2026-05-01T23:59:59.000Z",
  "is_active": true
}
```

#### Update Price Tier
```http
PUT /api/admin/event-zone-price/update
Authorization: Bearer <token>
Content-Type: application/json

{
  "event_zone_price_id": 1,
  "event_zone_id": 1,
  "price": 550000,
  "currency": "VND",
  "start_time": "2026-04-15T10:00:00.000Z",
  "end_time": "2026-05-01T23:59:59.000Z",
  "is_active": true
}
```

#### Delete Price Tier
```http
DELETE /api/admin/event-zone-price/delete
Authorization: Bearer <token>
Content-Type: application/json

{
  "event_zone_price_id": 1
}
```

---

### System Admin - Users

#### Create User
```http
POST /api/admin/sys-user/insert
Authorization: Bearer <token>
Content-Type: application/json

{
  "username": "newuser01",
  "email": "newuser@example.com",
  "phone": "0901234567",
  "password_hash": "hashed_password",
  "full_name": "Người Dùng Mới",
  "user_type": "user",
  "status": "active"
}
```

#### Update User
```http
PUT /api/admin/sys-user/update
Authorization: Bearer <token>
Content-Type: application/json

{
  "user_id": 5,
  "username": "newuser01",
  "email": "updated@example.com",
  ...
}
```

#### List Users
```http
GET /api/admin/sys-user/getpagedlist?pagesize=20&offset=0&keysearch=user&user_type=&status=active
Authorization: Bearer <token>
```

---

### System Admin - Roles

#### Create Role
```http
POST /api/admin/sys-role/insert
Authorization: Bearer <token>
Content-Type: application/json

{
  "role_code": "MODERATOR",
  "role_name": "Moderator",
  "description": "Moderates user content",
  "status": "active"
}
```

#### List Roles
```http
GET /api/admin/sys-role/getpagedlist?pagesize=20&offset=0
Authorization: Bearer <token>
```

---

## Client Endpoints

### Browse Events

#### Featured Events
```http
GET /api/client/event/featured?limit=8
```

**Response** (200 OK):
```json
{
  "issuccess": true,
  "status": "success",
  "message": "Lấy danh sách sự kiện nổi bật thành công",
  "data": [
    {
      "event_id": 123,
      "event_code": "EVT20260501",
      "event_name": "Sky Tour Live Concert",
      "description": "...",
      "venue_id": 1,
      "venue_name": "Quốc Ops Stadium",
      "city": "Hà Nội",
      "banner_url": "...",
      "start_time": "2026-05-15T19:00:00",
      "end_time": "2026-05-15T22:00:00",
      "status": "on_sale"
    }
  ]
}
```

#### Trending Events
```http
GET /api/client/event/trending?limit=12
```

#### Upcoming Events
```http
GET /api/client/event/upcoming?limit=12
```

#### Search Events
```http
GET /api/client/event/search?pagesize=20&offset=0&keysearch=sky&status=on_sale&venue_id=-1&from_date=&to_date=
```

#### Event Detail (with Seat Inventory)
```http
GET /api/client/event/{eventId}
```

**Response** (200 OK):
```json
{
  "issuccess": true,
  "status": "success",
  "message": "Lấy chi tiết sự kiện thành công",
  "data": {
    "event_id": 123,
    "event_name": "Sky Tour Live Concert",
    "venue_name": "Quốc Ops Stadium",
    "banner_url": "...",
    "start_time": "2026-05-15T19:00:00",
    "end_time": "2026-05-15T22:00:00",
    "zones": [
      {
        "event_zone_id": 1,
        "zone_name": "Sân",
        "current_price": 500000,
        "seats": [
          {
            "seat_id": 1001,
            "seat_label": "A-01",
            "status": "available"
          },
          {
            "seat_id": 1002,
            "seat_label": "A-02",
            "status": "held"
          }
        ]
      }
    ]
  }
}
```

---

### Booking (Seat Hold)

#### Hold Seats (10-minute reserve)
```http
POST /api/client/booking/hold-seats
Authorization: Bearer <token>
Content-Type: application/json
Idempotency-Key: "hold-event123-user1-001"

{
  "event_id": 123,
  "seat_ids": [1001, 1002, 1003]
}
```

**Response** (200 OK):
```json
{
  "issuccess": true,
  "status": "success",
  "message": "Giữ ghế thành công. Vui lòng hoàn tất thanh toán trong 10 phút",
  "data": {
    "hold_id": 500,
    "hold_code": "HOLD-abc123def456",
    "expires_at": "2026-05-15T14:15:30.000Z",
    "total_price": 1500000,
    "seats_held": 3
  }
}
```

#### Checkout (Convert Hold to Order)
```http
POST /api/client/booking/checkout
Authorization: Bearer <token>
Content-Type: application/json

{
  "hold_id": 500
}
```

**Response** (200 OK):
```json
{
  "issuccess": true,
  "status": "success",
  "message": "Tạo đơn hàng thành công",
  "data": {
    "order_id": 700,
    "order_code": "ORD-2026-001234",
    "total_price": 1500000,
    "payment_url": "https://payment-gateway.com/pay?ref=abc123",
    "payment_deadline": "2026-05-15T14:25:30.000Z"
  }
}
```

---

### Payment Callback (From Payment Provider)

#### Payment Confirmation
```http
POST /api/client/payment/callback
Content-Type: application/json

{
  "payment_ref": "PAY-abc123",
  "provider_transaction_ref": "VNPAY_TXN_001",
  "payment_status": "success",
  "amount": 1500000,
  "signature": "signature_hash_from_provider"
}
```

**Response** (200 OK):
```json
{
  "issuccess": true,
  "status": "success",
  "message": "Xác nhận thanh toán thành công"
}
```

---

### User Tickets

#### Get My Tickets
```http
GET /api/client/ticket/my-tickets?pagesize=20&offset=0&event_id=-1&status=issued
Authorization: Bearer <token>
```

**Response** (200 OK):
```json
{
  "issuccess": true,
  "status": "success",
  "data": [
    {
      "ticket_id": 1,
      "ticket_code": "TK2026-001234",
      "event_name": "Sky Tour Live Concert",
      "seat_label": "A-01",
      "seat_zone": "Sân",
      "status": "issued",
      "issued_at": "2026-05-15T13:00:00"
    }
  ]
}
```

---

### User Profile

#### Get My Profile
```http
GET /api/client/user/profile
Authorization: Bearer <token>
```

**Response** (200 OK):
```json
{
  "issuccess": true,
  "status": "success",
  "data": {
    "user_id": 1,
    "username": "johnsmith",
    "email": "john@example.com",
    "phone": "0901234567",
    "full_name": "John Smith",
    "user_type": "user",
    "created_at": "2026-01-01T10:00:00"
  }
}
```

---

## Auth Endpoints

### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "password123"
}
```

**Response** (200 OK):
```json
{
  "issuccess": true,
  "status": "success",
  "message": "Đăng nhập thành công",
  "data": {
    "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "token_type": "Bearer",
    "expires_in": 3600,
    "user_id": 1,
    "username": "admin",
    "full_name": "System Admin"
  }
}
```

### Logout (Client-side: Remove token from localStorage)

---

## Error Responses

### Validation Error (400)
```http
HTTP/1.1 400 Bad Request
Content-Type: application/json

{
  "issuccess": false,
  "status": "error",
  "message": "Validation failed",
  "errorcode": 400,
  "data": null,
  "errors": [
    {
      "property_message": "event_name",
      "error_message": "event_name là bắt buộc"
    },
    {
      "property_message": "start_time",
      "error_message": "start_time phải là ngày hợp lệ"
    }
  ]
}
```

### Unauthorized (401)
```http
HTTP/1.1 401 Unauthorized
Content-Type: application/json

{
  "issuccess": false,
  "status": "error",
  "message": "Token không hợp lệ hoặc đã hết hạn",
  "errorcode": 401,
  "data": null,
  "errors": []
}
```

### Business Logic Error (200 but status=error)
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "issuccess": false,
  "status": "error",
  "message": "Sự kiện không tìm thấy",
  "errorcode": 404,
  "data": null,
  "errors": []
}
```

### Server Error (500)
```http
HTTP/1.1 500 Internal Server Error
Content-Type: application/json

{
  "issuccess": false,
  "status": "error",
  "message": "Lỗi hệ thống: Database connection failed",
  "errorcode": 500,
  "data": null,
  "errors": []
}
```

---

## Rate Limiting & Best Practices

- **No explicit rate limiting** (use reverse proxy / API gateway if needed)
- **Pagination**: Always paginate list endpoints (default 20 items per page)
- **Caching**: 
  - Event details cached 5 minutes
  - Client event lists cached 3 minutes
  - Use `Cache-Control` headers in production
- **Idempotency**: Use `Idempotency-Key` header for POST requests to prevent duplicates
- **Timeouts**: 30-second timeout on all database operations

---

## Swagger/OpenAPI

Auto-generated documentation available at:
```
GET http://localhost:5000/swagger
```

All endpoints documented with descriptions, parameters, and responses.


