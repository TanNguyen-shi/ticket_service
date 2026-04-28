# Database Schema & Entities

## Overview

- **RDBMS**: PostgreSQL 14+
- **Schema**: `ticketing`
- **Access Method**: Dapper + Stored Procedures (No EF Core)
- **Mapping**: Npgsql → C# via custom reflection-based mapper (DTOs)
- **Soft Deletes**: Most entities have `is_deleted` flag (except inventory/hold/log tables)

---

## Entity Relationships Diagram

```
sys_user (1) ──────→ (M) sys_user_role
sys_role (1) ──────→ (M) sys_user_role

customer (1) ──────→ (M) seat_hold
customer (1) ──────→ (M) ticket_order
customer (1) ──────→ (M) ticket
customer (1) ──────→ (M) idempotency_request

venue (1) ──────→ (M) venue_section
venue_section (1) ──────→ (M) venue_seat
venue_seat (1) ──────→ (M) event_seat_inventory

event (1) ──────→ (M) event_zone
event_zone (1) ──────→ (M) event_zone_price
event_zone (1) ──────→ (M) event_zone_section
event_zone_section (M) ──────→ (1) venue_section

event (1) ──────→ (M) event_seat_inventory
event_zone (1) ──────→ (M) event_seat_inventory
event_seat_inventory (1) ──────→ (M) seat_hold_item
event_seat_inventory (1) ──────→ (M) ticket_order_item

seat_hold (1) ──────→ (M) seat_hold_item
seat_hold (1) ──────→ (1) ticket_order

ticket_order (1) ──────→ (M) ticket_order_item
ticket_order_item (1) ──────→ (1) ticket
ticket_order (1) ──────→ (M) payment_transaction
payment_transaction (1) ──────→ (M) payment_callback_log

event_publish_log (M) ──────→ (1) event
audit_log (M) ──────→ (1) sys_user
```

---

## Core Tables

### 1. System Admin Tables

#### `sys_user`
Admin và internal users (admin, customer mapping).

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| user_id | BIGSERIAL | PK | |
| username | VARCHAR(50) | UNIQUE (partial), NOT NULL | Login username |
| email | VARCHAR(255) | UNIQUE (partial), NULLABLE | Contact email |
| phone | VARCHAR(20) | UNIQUE (partial), NULLABLE | Contact phone |
| password_hash | VARCHAR(500) | NOT NULL | Hashed password |
| full_name | VARCHAR(255) | NOT NULL | Display name |
| user_type | VARCHAR(20) | NOT NULL | `'admin'`, `'customer'` |
| status | VARCHAR(20) | NOT NULL | `'active'`, `'inactive'`, `'locked'` |
| last_login_at | TIMESTAMP(3) | NULLABLE | |
| created_by | BIGINT | NULLABLE | FK to sys_user |
| created_at | TIMESTAMP(3) | DEFAULT now() | |
| updated_by | BIGINT | NULLABLE | FK to sys_user |
| updated_at | TIMESTAMP(3) | NULLABLE | |
| is_deleted | BOOLEAN | DEFAULT false | Soft delete |

**Check Constraints**: `status` ∈ `{active, inactive, locked}`, `user_type` ∈ `{admin, customer}`

**Indexes**:
- `ix_sys_user_status` on `(status)`
- `ix_sys_user_user_type` on `(user_type)`
- `ix_sys_user_username_status_deleted` on `(username, status, is_deleted)`
- `ux_sys_user_username` UNIQUE on `(username) WHERE is_deleted = false`
- `ux_sys_user_email` UNIQUE on `(email) WHERE email IS NOT NULL AND is_deleted = false`
- `ux_sys_user_phone` UNIQUE on `(phone) WHERE phone IS NOT NULL AND is_deleted = false`

---

#### `sys_role`
Roles: ADMIN, STAFF, USER, v.v.

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| role_id | BIGSERIAL | PK | |
| role_code | VARCHAR(50) | UNIQUE (partial), NOT NULL | `'ADMIN'`, `'STAFF'` |
| role_name | VARCHAR(100) | NOT NULL | Display name |
| description | VARCHAR(500) | NULLABLE | |
| status | VARCHAR(20) | NOT NULL | `'active'`, `'inactive'` |
| created_by | BIGINT | NULLABLE | FK to sys_user |
| created_at | TIMESTAMP(3) | DEFAULT now() | |
| updated_by | BIGINT | NULLABLE | FK to sys_user |
| updated_at | TIMESTAMP(3) | NULLABLE | |
| is_deleted | BOOLEAN | DEFAULT false | |

**Check Constraints**: `status` ∈ `{active, inactive}`

**Indexes**:
- `ix_sys_role_status` on `(status)`
- `ix_sys_role_role_code_deleted` on `(role_code, is_deleted)`
- `ux_sys_role_role_code` UNIQUE on `(role_code) WHERE is_deleted = false`

---

#### `sys_user_role`
Junction table: gán roles cho users.

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| user_role_id | BIGSERIAL | PK | |
| user_id | BIGINT | FK to sys_user, NOT NULL | |
| role_id | BIGINT | FK to sys_role, NOT NULL | |
| assigned_at | TIMESTAMP(3) | DEFAULT now() | |
| assigned_by | BIGINT | FK to sys_user, NULLABLE | |
| is_deleted | BOOLEAN | DEFAULT false | |

**Indexes**:
- `ix_sys_user_role_user_id` on `(user_id)`
- `ix_sys_user_role_role_id` on `(role_id)`
- `ux_sys_user_role_user_role` UNIQUE on `(user_id, role_id) WHERE is_deleted = false`

---

### 2. Customer Table

#### `customer`
End-user accounts (tách biệt với sys_user dành cho admin/staff).

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| customer_id | BIGSERIAL | PK | |
| customer_code | VARCHAR(50) | UNIQUE, NOT NULL | Mã khách hàng |
| username | VARCHAR(50) | UNIQUE (partial), NOT NULL | Login username |
| email | VARCHAR(100) | UNIQUE (partial), NULLABLE | |
| phone | VARCHAR(20) | NULLABLE | |
| password_hash | VARCHAR(255) | NOT NULL | |
| full_name | VARCHAR(100) | NOT NULL | |
| avatar_url | VARCHAR(255) | NULLABLE | |
| status | VARCHAR(20) | DEFAULT 'active', NOT NULL | `'active'`, `'inactive'` |
| email_verified | BOOLEAN | DEFAULT false | |
| last_login_at | TIMESTAMP | NULLABLE | |
| created_at | TIMESTAMP | DEFAULT now() | |
| updated_at | TIMESTAMP | NULLABLE | |
| is_deleted | BOOLEAN | DEFAULT false | |

**Indexes**:
- `idx_customer_status` on `(status)`
- `idx_customer_username` UNIQUE on `(username) WHERE is_deleted = false`
- `idx_customer_email` UNIQUE on `(email) WHERE is_deleted = false AND email IS NOT NULL`

---

### 3. Venue Tables

#### `venue`
Địa điểm tổ chức sự kiện (nhà hát, sân vận động).

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| venue_id | BIGSERIAL | PK | |
| venue_code | VARCHAR(50) | UNIQUE (partial), NOT NULL | `'VN001'` |
| venue_name | VARCHAR(255) | NOT NULL | |
| address_line | VARCHAR(500) | NULLABLE | |
| city | VARCHAR(100) | NULLABLE | |
| country | VARCHAR(100) | NULLABLE | |
| status | VARCHAR(20) | NOT NULL | `'active'`, `'inactive'` |
| created_by | BIGINT | FK to sys_user, NULLABLE | |
| created_at | TIMESTAMP(3) | DEFAULT now() | |
| updated_by | BIGINT | FK to sys_user, NULLABLE | |
| updated_at | TIMESTAMP(3) | NULLABLE | |
| is_deleted | BOOLEAN | DEFAULT false | |

**Check Constraints**: `status` ∈ `{active, inactive}`

**Indexes**:
- `ix_venue_status` on `(status)`
- `ix_venue_name` on `(venue_name)`
- `ix_venue_client_basic_lookup` on `(venue_id, venue_name, city)`
- `ux_venue_code` UNIQUE on `(venue_code) WHERE is_deleted = false`

---

#### `venue_section`
Khu vực trong venue (Block A, VIP, Orchestra).

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| section_id | BIGSERIAL | PK | |
| venue_id | BIGINT | FK to venue, NOT NULL | |
| section_code | VARCHAR(50) | UNIQUE (partial), NOT NULL | `'SEA'`, `'SEVIP'` |
| section_name | VARCHAR(255) | NOT NULL | |
| display_order | INT | DEFAULT 0 | Thứ tự hiển thị |
| status | VARCHAR(20) | NOT NULL | `'active'`, `'inactive'` |
| created_by | BIGINT | FK to sys_user, NULLABLE | |
| created_at | TIMESTAMP(3) | DEFAULT now() | |
| updated_by | BIGINT | FK to sys_user, NULLABLE | |
| updated_at | TIMESTAMP(3) | NULLABLE | |
| is_deleted | BOOLEAN | DEFAULT false | |

**Check Constraints**: `status` ∈ `{active, inactive}`

**Indexes**:
- `ix_venue_section_venue_id` on `(venue_id)`
- `ix_venue_section_venue_display_order` on `(venue_id, display_order)`
- `ux_venue_section_venue_code` UNIQUE on `(venue_id, section_code) WHERE is_deleted = false`

---

#### `venue_seat`
Ghế ngồi riêng lẻ trong section.

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| seat_id | BIGSERIAL | PK | |
| venue_id | BIGINT | FK to venue (via section), NOT NULL | |
| section_id | BIGINT | FK to venue_section, NOT NULL | |
| seat_code | VARCHAR(50) | UNIQUE (partial), NOT NULL | `'A01'`, `'A02'` |
| row_label | VARCHAR(20) | NULLABLE | `'A'`, `'B'` |
| seat_number | VARCHAR(20) | NULLABLE | `'1'`, `'2'` |
| seat_label | VARCHAR(50) | NULLABLE | Display: `'A-01'` |
| x_pos | NUMERIC(10,2) | NULLABLE | Tọa độ X trên sơ đồ |
| y_pos | NUMERIC(10,2) | NULLABLE | Tọa độ Y trên sơ đồ |
| seat_type | VARCHAR(20) | NOT NULL | `'seat'`, `'standing'`, `'table'` |
| status | VARCHAR(20) | NOT NULL | `'active'`, `'inactive'` |
| created_by | BIGINT | FK to sys_user, NULLABLE | |
| created_at | TIMESTAMP(3) | DEFAULT now() | |
| updated_by | BIGINT | FK to sys_user, NULLABLE | |
| updated_at | TIMESTAMP(3) | NULLABLE | |
| is_deleted | BOOLEAN | DEFAULT false | |

**Check Constraints**: `seat_type` ∈ `{seat, standing, table}`, `status` ∈ `{active, inactive}`

**Indexes**:
- `ix_venue_seat_section_id` on `(section_id)`
- `ix_venue_seat_venue_id` on `(venue_id)`
- `ix_venue_seat_venue_section_status_deleted` on `(venue_id, section_id, status, is_deleted)`
- `ux_venue_seat_venue_seat_code` UNIQUE on `(venue_id, seat_code) WHERE is_deleted = false`

---

### 4. Event Tables

#### `event`
Sự kiện bán vé.

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| event_id | BIGSERIAL | PK | |
| event_code | VARCHAR(50) | NOT NULL | `'EVT001'` |
| event_name | VARCHAR(255) | NOT NULL | |
| description | TEXT | NULLABLE | |
| venue_id | BIGINT | FK to venue, NOT NULL | |
| banner_url | VARCHAR(1000) | NULLABLE | Poster sự kiện |
| start_time | TIMESTAMP(3) | NOT NULL | Thời điểm bắt đầu |
| end_time | TIMESTAMP(3) | NOT NULL | Thời điểm kết thúc |
| sale_start_time | TIMESTAMP(3) | NULLABLE | Bắt đầu bán vé |
| sale_end_time | TIMESTAMP(3) | NULLABLE | Kết thúc bán vé |
| status | VARCHAR(30) | NOT NULL | `'draft'`, `'published'`, `'on_sale'`, `'sale_closed'`, `'ended'`, `'cancelled'` |
| published_at | TIMESTAMP(3) | NULLABLE | |
| on_sale_at | TIMESTAMP(3) | NULLABLE | |
| is_featured | BOOLEAN | DEFAULT false | Hiển thị mục nổi bật |
| is_trending | BOOLEAN | DEFAULT false | Hiển thị mục trending |
| display_order | INT | DEFAULT 0 | Thứ tự hiển thị |
| created_by | BIGINT | NULLABLE | FK to sys_user |
| created_at | TIMESTAMP(3) | DEFAULT now() | |
| updated_by | BIGINT | NULLABLE | FK to sys_user |
| updated_at | TIMESTAMP(3) | NULLABLE | |
| is_deleted | BOOLEAN | DEFAULT false | |

**Indexes**:
- `ix_event_status` on `(status)`
- `ix_event_featured_trending` on `(is_featured, is_trending)`
- `ix_event_display_order` on `(display_order)`
- `ix_event_start_end_time` on `(start_time, end_time)`
- `ix_event_status_sale_time` on `(status, sale_start_time, sale_end_time)`
- `ix_event_venue_id` on `(venue_id)`
- `ix_event_client_homepage_featured` on `(is_deleted, is_featured, status, display_order, start_time, event_id)`
- `ix_event_client_homepage_trending` on `(is_deleted, is_trending, status, display_order, start_time, event_id)`
- `ix_event_client_homepage_upcoming` on `(is_deleted, status, start_time, is_featured, is_trending, display_order, event_id)`

---

#### `event_zone`
Khu vực định giá trong sự kiện (Floor, VIP, Premium).

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| event_zone_id | BIGSERIAL | PK | |
| event_id | BIGINT | FK to event, NOT NULL | |
| zone_code | VARCHAR(50) | NOT NULL | `'FLOOR'`, `'VIP'` |
| zone_name | VARCHAR(100) | NOT NULL | |
| color_hex | VARCHAR(20) | NULLABLE | Màu hiển thị trên sơ đồ |
| description | VARCHAR(500) | NULLABLE | |
| display_order | INT | DEFAULT 0 | |
| status | VARCHAR(20) | NOT NULL | `'active'`, `'inactive'` |
| created_by | BIGINT | NULLABLE | FK to sys_user |
| created_at | TIMESTAMP(3) | DEFAULT now() | |
| updated_by | BIGINT | NULLABLE | FK to sys_user |
| updated_at | TIMESTAMP(3) | NULLABLE | |
| is_deleted | BOOLEAN | DEFAULT false | |

**Indexes**:
- `ix_event_zone_event_id` on `(event_id)`
- `ix_event_zone_event_display_order` on `(event_id, display_order)`

---

#### `event_zone_price`
Các mức giá theo thời gian của từng zone.

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| event_zone_price_id | BIGSERIAL | PK | |
| event_zone_id | BIGINT | FK to event_zone, NOT NULL | |
| price | NUMERIC(18,2) | NOT NULL | Giá vé |
| currency | VARCHAR(10) | DEFAULT 'VND' | |
| start_time | TIMESTAMP(3) | NULLABLE | Giá áp dụng từ (early bird) |
| end_time | TIMESTAMP(3) | NULLABLE | Giá áp dụng đến |
| is_active | BOOLEAN | DEFAULT true | |
| created_by | BIGINT | NULLABLE | FK to sys_user |
| created_at | TIMESTAMP(3) | DEFAULT now() | |
| updated_by | BIGINT | NULLABLE | FK to sys_user |
| updated_at | TIMESTAMP(3) | NULLABLE | |
| is_deleted | BOOLEAN | DEFAULT false | |

**Indexes**:
- `ix_event_zone_price_event_zone_id` on `(event_zone_id)`
- `ix_event_zone_price_active_deleted` on `(event_zone_id, is_active, is_deleted)`
- `ix_event_zone_price_time_range` on `(start_time, end_time)`

---

#### `event_zone_section`
Map zone sự kiện → section vật lý của venue.

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| event_zone_section_id | BIGSERIAL | PK | |
| event_id | BIGINT | FK to event, NOT NULL | |
| event_zone_id | BIGINT | FK to event_zone, NOT NULL | |
| section_id | BIGINT | FK to venue_section, NOT NULL | |
| created_by | BIGINT | FK to sys_user, NULLABLE | |
| created_at | TIMESTAMP(3) | DEFAULT now() | |
| updated_by | BIGINT | FK to sys_user, NULLABLE | |
| updated_at | TIMESTAMP(3) | NULLABLE | |
| is_deleted | BOOLEAN | DEFAULT false | |

**Unique Constraint**: `UNIQUE (event_id, event_zone_id, section_id) WHERE is_deleted = false`

**Indexes**:
- `ix_event_zone_section_event_id` on `(event_id)`
- `ix_event_zone_section_event_zone_id` on `(event_zone_id)`
- `ix_event_zone_section_section_id` on `(section_id)`
- `ix_event_zone_section_event_zone_section` on `(event_id, event_zone_id, section_id)`

---

#### `event_seat_inventory`
Inventory theo thời gian thực cho từng ghế mỗi sự kiện.

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| event_seat_inventory_id | BIGSERIAL | PK | |
| event_id | BIGINT | FK to event, NOT NULL | |
| seat_id | BIGINT | FK to venue_seat, NOT NULL | |
| event_zone_id | BIGINT | FK to event_zone, NOT NULL | |
| seat_status | VARCHAR(20) | NOT NULL | `'available'`, `'held'`, `'sold'`, `'locked'`, `'unavailable'` |
| current_hold_id | BIGINT | FK to seat_hold, NULLABLE | Hold hiện tại (nếu đang held) |
| current_order_item_id | BIGINT | FK to ticket_order_item, NULLABLE | Order item (nếu đã sold) |
| base_price | NUMERIC(18,2) | NULLABLE | Giá snapshot tại thời điểm khởi tạo |
| version | INT | DEFAULT 1 | Optimistic locking |
| updated_at | TIMESTAMP(3) | DEFAULT now() | |

**Check Constraints**: `seat_status` ∈ `{available, held, sold, locked, unavailable}`, `version >= 1`, `base_price >= 0`

**Unique Constraint**: `UNIQUE (event_id, seat_id)`

**Indexes**:
- `ix_event_seat_inventory_event_status` on `(event_id, seat_status)`
- `ix_event_seat_inventory_event_zone_status` on `(event_id, event_zone_id, seat_status)`
- `ix_event_seat_inventory_current_hold_id` on `(current_hold_id)`
- `ix_event_seat_inventory_current_order_item_id` on `(current_order_item_id)`

---

### 5. Seat Hold (Reservation) Tables

#### `seat_hold`
Hold tạm thời trước khi thanh toán.

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| hold_id | BIGSERIAL | PK | |
| hold_code | VARCHAR(50) | UNIQUE, NOT NULL | `'HOLD-UUID'` |
| event_id | BIGINT | FK to event, NOT NULL | |
| customer_id | BIGINT | FK to customer, NOT NULL | |
| status | VARCHAR(20) | NOT NULL | `'active'`, `'expired'`, `'released'`, `'converted'`, `'cancelled'` |
| hold_started_at | TIMESTAMP(3) | NOT NULL | |
| hold_expires_at | TIMESTAMP(3) | NOT NULL | Thường now() + 10 phút |
| released_at | TIMESTAMP(3) | NULLABLE | Khi release thủ công |
| release_reason | VARCHAR(100) | NULLABLE | `'user_request'`, `'payment_failed'`, `'auto_expire'` |
| created_at | TIMESTAMP(3) | DEFAULT now() | |

**Check Constraints**: `status` ∈ `{active, expired, released, converted, cancelled}`, `hold_expires_at > hold_started_at`

**Indexes**:
- `ix_seat_hold_event_id` on `(event_id)`
- `ix_seat_hold_customer_id` on `(customer_id)`
- `ix_seat_hold_event_customer_status` on `(event_id, customer_id, status)`
- `ix_seat_hold_event_status_expires_at` on `(event_id, status, hold_expires_at)`
- `ix_seat_hold_status_expires_at` on `(status, hold_expires_at)`
- `ux_seat_hold_code` UNIQUE on `(hold_code)`

---

#### `seat_hold_item`
Ghế riêng lẻ trong một hold.

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| hold_item_id | BIGSERIAL | PK | |
| hold_id | BIGINT | FK to seat_hold, NOT NULL | |
| event_seat_inventory_id | BIGINT | FK to event_seat_inventory, NOT NULL | |
| seat_id | BIGINT | FK to venue_seat, NOT NULL | |
| zone_id | BIGINT | FK to event_zone, NOT NULL | |
| price_at_hold | NUMERIC(18,2) | NOT NULL | Giá lock tại thời điểm hold |
| seat_label_snapshot | VARCHAR(50) | NULLABLE | `'A-01'` (lịch sử) |
| zone_name_snapshot | VARCHAR(100) | NULLABLE | `'VIP'` (lịch sử) |
| status | VARCHAR(20) | NOT NULL | `'active'`, `'released'`, `'converted'`, `'expired'` |
| created_at | TIMESTAMP(3) | DEFAULT now() | |

**Check Constraints**: `status` ∈ `{active, released, converted, expired}`, `price_at_hold >= 0`

**Unique Constraint**: `UNIQUE (hold_id, event_seat_inventory_id)`

**Indexes**:
- `ix_seat_hold_item_hold_id` on `(hold_id)`
- `ix_seat_hold_item_inventory_id` on `(event_seat_inventory_id)`
- `ix_seat_hold_item_seat_id` on `(seat_id)`
- `ix_seat_hold_item_zone_id` on `(zone_id)`

---

### 6. Order & Ticket Tables

#### `ticket_order`
Đơn hàng / checkout cart.

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| order_id | BIGSERIAL | PK | |
| order_code | VARCHAR(50) | UNIQUE, NOT NULL | `'ORD-{guid}'` |
| event_id | BIGINT | FK to event, NOT NULL | |
| customer_id | BIGINT | FK to customer, NOT NULL | |
| hold_id | BIGINT | FK to seat_hold, NULLABLE, UNIQUE | Nguồn hold |
| total_amount | NUMERIC(18,2) | NOT NULL | Tổng giá trước giảm |
| discount_amount | NUMERIC(18,2) | DEFAULT 0 | Tiền giảm giá |
| final_amount | NUMERIC(18,2) | NOT NULL | Số tiền thực thanh toán |
| order_status | VARCHAR(30) | NOT NULL | `'pending_payment'`, `'paid'`, `'payment_failed'`, `'cancelled'`, `'expired'` |
| expired_at | TIMESTAMP(3) | NULLABLE | Hạn thanh toán |
| paid_at | TIMESTAMP(3) | NULLABLE | Khi xác nhận thanh toán |
| created_at | TIMESTAMP(3) | DEFAULT now() | |
| updated_at | TIMESTAMP(3) | NULLABLE | |

**Check Constraints**: `order_status` ∈ `{pending_payment, paid, payment_failed, cancelled, expired}`, `total_amount >= 0`, `discount_amount >= 0`, `final_amount >= 0`

**Indexes**:
- `ix_ticket_order_customer_id` on `(customer_id)`
- `ix_ticket_order_event_id` on `(event_id)`
- `ix_ticket_order_hold_id` on `(hold_id)`
- `ix_ticket_order_customer_created_at_desc` on `(customer_id, created_at DESC)`
- `ix_ticket_order_status_expired_at` on `(order_status, expired_at)`
- `ux_ticket_order_code` UNIQUE on `(order_code)`
- `ux_ticket_order_hold_id` UNIQUE on `(hold_id) WHERE hold_id IS NOT NULL`

---

#### `ticket_order_item`
Ghế riêng lẻ trong đơn hàng.

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| order_item_id | BIGSERIAL | PK | |
| order_id | BIGINT | FK to ticket_order, NOT NULL | |
| event_seat_inventory_id | BIGINT | FK to event_seat_inventory, NOT NULL | |
| seat_id | BIGINT | FK to venue_seat, NOT NULL | |
| zone_id | BIGINT | FK to event_zone, NOT NULL | |
| unit_price | NUMERIC(18,2) | NOT NULL | Giá cho ghế này |
| seat_label_snapshot | VARCHAR(50) | NULLABLE | `'A-01'` |
| zone_name_snapshot | VARCHAR(100) | NULLABLE | `'VIP'` |
| item_status | VARCHAR(20) | NOT NULL | `'pending'`, `'paid'`, `'cancelled'` |
| created_at | TIMESTAMP(3) | DEFAULT now() | |

**Check Constraints**: `item_status` ∈ `{pending, paid, cancelled}`, `unit_price >= 0`

**Unique Constraint**: `UNIQUE (order_id, event_seat_inventory_id)`

**Indexes**:
- `ix_ticket_order_item_order_id` on `(order_id)`
- `ix_ticket_order_item_inventory_id` on `(event_seat_inventory_id)`
- `ix_ticket_order_item_seat_id` on `(seat_id)`
- `ix_ticket_order_item_zone_id` on `(zone_id)`

---

#### `ticket`
Vé được phát sau khi thanh toán xác nhận.

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| ticket_id | BIGSERIAL | PK | |
| ticket_code | VARCHAR(100) | UNIQUE, NOT NULL | QR code / barcode |
| order_item_id | BIGINT | FK to ticket_order_item, UNIQUE | |
| event_id | BIGINT | FK to event, NOT NULL | |
| customer_id | BIGINT | FK to customer, NOT NULL | |
| seat_id | BIGINT | FK to venue_seat, NOT NULL | |
| seat_label_snapshot | VARCHAR(50) | NULLABLE | `'A-01'` |
| zone_name_snapshot | VARCHAR(100) | NULLABLE | `'VIP'` |
| event_name_snapshot | VARCHAR(255) | NULLABLE | Tên sự kiện lúc mua |
| ticket_status | VARCHAR(20) | NOT NULL | `'active'`, `'used'`, `'cancelled'`, `'refunded'` |
| issued_at | TIMESTAMP(3) | NOT NULL | |
| checked_in_at | TIMESTAMP(3) | NULLABLE | Khi quét tại cổng |

**Check Constraints**: `ticket_status` ∈ `{active, used, cancelled, refunded}`

**Indexes**:
- `ix_ticket_event_id` on `(event_id)`
- `ix_ticket_customer_id` on `(customer_id)`
- `ix_ticket_customer_event` on `(customer_id, event_id)`
- `ix_ticket_status` on `(ticket_status)`
- `ux_ticket_code` UNIQUE on `(ticket_code)`
- `ux_ticket_order_item_id` UNIQUE on `(order_item_id)`

---

### 7. Payment Tables

#### `payment_transaction`
Bản ghi thanh toán.

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| payment_id | BIGSERIAL | PK | |
| order_id | BIGINT | FK to ticket_order, NOT NULL | |
| payment_provider | VARCHAR(30) | NOT NULL | `'vnpay'`, `'momo'`, `'mock'` |
| payment_ref | VARCHAR(100) | UNIQUE, NOT NULL | Internal reference |
| provider_transaction_ref | VARCHAR(100) | UNIQUE (partial), NULLABLE | External transaction ID |
| amount | NUMERIC(18,2) | NOT NULL | Số tiền thanh toán |
| payment_status | VARCHAR(20) | NOT NULL | `'initiated'`, `'pending'`, `'success'`, `'failed'`, `'cancelled'` |
| requested_at | TIMESTAMP(3) | NOT NULL | Khi yêu cầu thanh toán |
| confirmed_at | TIMESTAMP(3) | NULLABLE | Khi provider xác nhận |
| raw_request_payload | TEXT | NULLABLE | Request JSON |
| raw_callback_payload | TEXT | NULLABLE | Callback JSON |
| created_at | TIMESTAMP(3) | DEFAULT now() | |
| updated_at | TIMESTAMP(3) | NULLABLE | |

**Check Constraints**: `payment_provider` ∈ `{vnpay, momo, mock}`, `payment_status` ∈ `{initiated, pending, success, failed, cancelled}`, `amount > 0`

**Indexes**:
- `ix_payment_transaction_order_id` on `(order_id)`
- `ix_payment_transaction_order_status` on `(order_id, payment_status)`
- `ix_payment_transaction_provider_transaction_ref` on `(provider_transaction_ref)`
- `ux_payment_transaction_payment_ref` UNIQUE on `(payment_ref)`
- `ux_payment_transaction_provider_ref` UNIQUE on `(provider_transaction_ref) WHERE provider_transaction_ref IS NOT NULL`

---

#### `payment_callback_log`
Audit trail của payment callbacks.

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| callback_log_id | BIGSERIAL | PK | |
| payment_id | BIGINT | FK to payment_transaction, NULLABLE | |
| payment_provider | VARCHAR(30) | NOT NULL | |
| external_transaction_ref | VARCHAR(100) | NULLABLE | |
| callback_signature | VARCHAR(500) | NULLABLE | Chữ ký để validate |
| payload | TEXT | NOT NULL | Raw callback body |
| signature_valid | BOOLEAN | NOT NULL | Chữ ký có hợp lệ không |
| processed_status | VARCHAR(20) | NOT NULL | `'received'`, `'processed'`, `'ignored'`, `'failed'` |
| received_at | TIMESTAMP(3) | DEFAULT now() | |
| processed_at | TIMESTAMP(3) | NULLABLE | |

**Check Constraints**: `payment_provider` ∈ `{vnpay, momo, mock}`, `processed_status` ∈ `{received, processed, ignored, failed}`

**Indexes**:
- `ix_payment_callback_log_payment_id` on `(payment_id)`
- `ix_payment_callback_log_external_transaction_ref` on `(external_transaction_ref)`
- `ix_payment_callback_log_provider_external_ref` on `(payment_provider, external_transaction_ref)`
- `ix_payment_callback_log_status_received_at` on `(processed_status, received_at)`

---

### 8. Idempotency & Audit Tables

#### `idempotency_request`
Ngăn chặn các thao tác trùng lặp.

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| idempotency_id | BIGSERIAL | PK | |
| idempotency_key | VARCHAR(100) | NOT NULL | Client-provided unique ID |
| request_type | VARCHAR(50) | NOT NULL | `'hold_seats'`, `'checkout'`, `'payment_callback'` |
| customer_id | BIGINT | FK to customer, NULLABLE | |
| request_hash | VARCHAR(200) | NULLABLE | Hash của request body |
| status | VARCHAR(20) | NOT NULL | `'processing'`, `'completed'`, `'failed'`, `'expired'` |
| response_snapshot | TEXT | NULLABLE | Response JSON nếu completed |
| created_at | TIMESTAMP(3) | DEFAULT now() | |
| expired_at | TIMESTAMP(3) | NULLABLE | Tự xóa sau thời điểm này |

**Check Constraints**: `status` ∈ `{processing, completed, failed, expired}`, `request_type` ∈ `{hold_seats, checkout, payment_callback}`

**Unique Constraint**: `UNIQUE (request_type, idempotency_key)`

**Indexes**:
- `ix_idempotency_request_customer_id` on `(customer_id)`
- `ix_idempotency_request_status_expired_at` on `(status, expired_at)`

---

#### `event_publish_log`
Lịch sử thay đổi trạng thái sự kiện.

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| event_publish_log_id | BIGSERIAL | PK | |
| event_id | BIGINT | FK to event, NOT NULL | |
| action | VARCHAR(30) | NOT NULL | `'publish'`, `'open_sale'`, `'close_sale'`, `'cancel'` |
| old_status | VARCHAR(30) | NULLABLE | Trạng thái trước |
| new_status | VARCHAR(30) | NOT NULL | Trạng thái sau |
| changed_by | BIGINT | FK to sys_user, NULLABLE | |
| changed_at | TIMESTAMP(3) | DEFAULT now() | |
| note | VARCHAR(1000) | NULLABLE | Ghi chú admin |

**Check Constraints**: `action` ∈ `{publish, open_sale, close_sale, cancel}`, `old_status` và `new_status` ∈ `{draft, published, on_sale, sale_closed, ended, cancelled}`

**Indexes**:
- `ix_event_publish_log_event_id` on `(event_id)`
- `ix_event_publish_log_event_changed_at_desc` on `(event_id, changed_at DESC)`

---

#### `audit_log`
Audit trail đầy đủ cho mọi thay đổi.

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| audit_log_id | BIGSERIAL | PK | |
| actor_user_id | BIGINT | FK to sys_user, NULLABLE | Ai thực hiện |
| module_name | VARCHAR(50) | NOT NULL | `'Event'`, `'Venue'`, `'User'` |
| action_name | VARCHAR(50) | NOT NULL | `'INSERT'`, `'UPDATE'`, `'DELETE'` |
| entity_name | VARCHAR(50) | NOT NULL | `'Event'`, `'EventZone'` |
| entity_id | BIGINT | NULLABLE | ID của entity bị thay đổi |
| old_data | TEXT | NULLABLE | JSON trước (với UPDATE) |
| new_data | TEXT | NULLABLE | JSON sau (với INSERT/UPDATE) |
| ip_address | VARCHAR(50) | NULLABLE | Request IP |
| created_at | TIMESTAMP(3) | DEFAULT now() | |

**Indexes**:
- `ix_audit_log_actor_user_id` on `(actor_user_id)`
- `ix_audit_log_entity_lookup` on `(entity_name, entity_id, created_at DESC)`
- `ix_audit_log_module_created_at_desc` on `(module_name, created_at DESC)`

---

## Critical Indexes

| Table | Columns | Reason |
|-------|---------|--------|
| `event_seat_inventory` | `(event_id, seat_status)` | Real-time inventory queries |
| `event_seat_inventory` | `(event_id, event_zone_id, seat_status)` | Zone-level inventory |
| `event` | `(is_deleted, status, start_time, ...)` | Homepage queries |
| `seat_hold` | `(event_id, status, hold_expires_at)` | Expiry cleanup jobs |
| `seat_hold` | `(status, hold_expires_at)` | Global expiry sweep |
| `ticket_order` | `(customer_id, created_at DESC)` | Customer's orders |
| `ticket` | `(customer_id, event_id)` | Customer's tickets for event |
| `audit_log` | `(entity_name, entity_id, created_at DESC)` | Change history lookup |
| `event_zone_price` | `(event_zone_id, is_active, is_deleted)` | Pricing queries |

---

## Data Types & Conventions

| Type | Usage | Notes |
|------|-------|-------|
| `BIGSERIAL` | Primary keys | Auto-increment long cho high-volume tables |
| `NUMERIC(18,2)` | Tiền tệ | Luôn dùng cho số tiền (tránh FLOAT) |
| `VARCHAR(n)` | Text | Dùng độ dài phù hợp |
| `TEXT` | Long text | JSON payloads, mô tả dài |
| `TIMESTAMP(3) without time zone` | Datetime | Lưu theo UTC, precision 3ms |
| `BOOLEAN` | Flags | `is_deleted`, `is_active`, `is_featured` |

---

## Migration Strategy

**Current Approach**:
- Manual SQL scripts in `database/functions/` and `database/schema/`
- Run on deployment (Docker compose setup)
- No automatic rollback (manual intervention required)

**Future Improvement**:
- Consider Flyway or similar version control

---

## Stored Procedures Pattern

All data access via PostgreSQL functions returning `refcursor`:

```sql
CREATE OR REPLACE FUNCTION ticketing.{table}_{action}(p_param1 type, p_param2 type, ...)
RETURNS refcursor
AS $function$
DECLARE
    v_out refcursor := '{table}_{action}';
BEGIN
    OPEN v_out FOR
    SELECT ... FROM ...;
    RETURN v_out;
END;
$function$;
```

**Standard Actions**:
- `insert(params)` → Returns refcursor with `{table}_id`
- `update(id, params)` → Returns refcursor with `{table}_id`
- `delete(id, updated_by)` → Returns refcursor with `{table}_id`
- `getbyid(id)` → Returns refcursor with full row + joined details
- `getpagedlist(pagesize, offset, filters)` → Returns refcursor with rows + `row_index`, `row_total`
