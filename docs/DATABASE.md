# Database Schema & Entities

## Overview

- **RDBMS**: PostgreSQL 14+
- **Schema**: `ticketing`
- **Access Method**: Dapper + Stored Procedures (No EF Core)
- **Mapping**: Npgsql → C# via custom reflection-based mapper (DTOs)
- **Soft Deletes**: All entities have `is_deleted` flag

---

## Entity Relationships Diagram

```
SysUser (1) ──────→ (M) SysUserRole
SysRole (1) ──────→ (M) SysUserRole

Venue (1) ──────→ (M) VenueSection
VenueSection (1) ──────→ (M) VenueSeat

Event (1) ──────→ (M) EventZone
Event (1) ──────→ (M) EventZonePrice
Event (1) ──────→ (M) EventZoneSection
EventZone (1) ──────→ (M) EventZonePrice
EventZoneSection (1) ──────→ (1) VenueSection

Event (1) ──────→ (M) EventSeatInventory
EventSeatInventory (1) ──────→ (M) SeatHold (via SeatHoldItem)
EventSeatInventory (1) ──────→ (M) TicketOrderItem

SeatHold (1) ──────→ (M) SeatHoldItem
SeatHoldItem (M) ──────→ (1) EventSeatInventory

TicketOrder (1) ──────→ (M) TicketOrderItem
TicketOrderItem (1) ──────→ (1) Ticket
TicketOrderItem (M) ──────→ (1) EventSeatInventory

TicketOrder (1) ──────→ (M) PaymentTransaction
PaymentTransaction (1) ──────→ (M) PaymentCallbackLog

EventPublishLog (M) ──────→ (1) Event
AuditLog (M) ──────→ (1) SysUser
```

---

## Core Tables

### 1. System Admin Tables

#### `sys_user`
Creator: admin, User, Staff

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| user_id | BIGSERIAL | PK | |
| username | VARCHAR(50) | UNIQUE, NOT NULL | Login username |
| email | VARCHAR(100) | UNIQUE, NULLABLE | Contact email |
| phone | VARCHAR(20) | NULLABLE | Contact phone |
| password_hash | VARCHAR(255) | NOT NULL | Bcrypt/Argon2 hash |
| full_name | VARCHAR(100) | NOT NULL | Display name |
| user_type | VARCHAR(20) | NOT NULL | 'admin', 'staff', 'user' |
| status | VARCHAR(20) | NOT NULL | 'active', 'inactive' |
| last_login_at | TIMESTAMP | NULLABLE | Last login datetime |
| created_by | BIGINT | FK to sys_user | |
| created_at | TIMESTAMP | DEFAULT now() | |
| updated_by | BIGINT | FK to sys_user | |
| updated_at | TIMESTAMP | NULLABLE | |
| is_deleted | BOOLEAN | DEFAULT false | Soft delete |

#### `sys_role`
Creator: admin, Roles (ADMIN, STAFF, USER)

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| role_id | BIGSERIAL | PK | |
| role_code | VARCHAR(50) | UNIQUE, NOT NULL | 'ADMIN', 'STAFF', 'USER' |
| role_name | VARCHAR(100) | NOT NULL | Display name |
| description | TEXT | NULLABLE | |
| status | VARCHAR(20) | NOT NULL | 'active', 'inactive' |
| created_by | BIGINT | FK to sys_user | |
| created_at | TIMESTAMP | DEFAULT now() | |
| updated_by | BIGINT | FK to sys_user | |
| updated_at | TIMESTAMP | NULLABLE | |
| is_deleted | BOOLEAN | DEFAULT false | |

#### `sys_user_role`
Junction table: assign roles to users

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| user_role_id | BIGSERIAL | PK | |
| user_id | BIGINT | FK to sys_user | |
| role_id | BIGINT | FK to sys_role | |
| assigned_at | TIMESTAMP | NOT NULL | |
| assigned_by | BIGINT | FK to sys_user | |
| is_deleted | BOOLEAN | DEFAULT false | |

---

### 2. Venue Tables

#### `venue`
Physical venues (concert halls, stadiums)

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| venue_id | BIGSERIAL | PK | |
| venue_code | VARCHAR(50) | UNIQUE, NOT NULL | 'VN001', 'VN002' |
| venue_name | VARCHAR(100) | NOT NULL | |
| address_line | VARCHAR(255) | NOT NULL | |
| city | VARCHAR(100) | NOT NULL | |
| country | VARCHAR(100) | NOT NULL | |
| capacity | INT | NULLABLE | Total seat count |
| admin_email | VARCHAR(100) | NULLABLE | Contact |
| status | VARCHAR(20) | NOT NULL | 'active', 'inactive' |
| created_by | BIGINT | FK to sys_user | |
| created_at | TIMESTAMP | DEFAULT now() | |
| updated_by | BIGINT | FK to sys_user | |
| updated_at | TIMESTAMP | NULLABLE | |
| is_deleted | BOOLEAN | DEFAULT false | |

**Indexes**:
- `venue_id`, `venue_code`

#### `venue_section`
Sections within a venue (Block A, Block B, VIP, Orchestra)

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| section_id | BIGSERIAL | PK | |
| venue_id | BIGINT | FK to venue | |
| section_code | VARCHAR(50) | UNIQUE, NOT NULL | 'SEA', 'SEB', 'SEVIP' |
| section_name | VARCHAR(100) | NOT NULL | |
| total_seats | INT | NOT NULL | |
| created_by | BIGINT | FK to sys_user | |
| created_at | TIMESTAMP | DEFAULT now() | |
| updated_by | BIGINT | FK to sys_user | |
| updated_at | TIMESTAMP | NULLABLE | |
| is_deleted | BOOLEAN | DEFAULT false | |

**Indexes**:
- `(venue_id, section_id)`

#### `venue_seat`
Individual seats

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| seat_id | BIGSERIAL | PK | |
| section_id | BIGINT | FK to venue_section | |
| seat_code | VARCHAR(50) | NOT NULL | 'A01', 'A02' |
| row_label | VARCHAR(20) | NOT NULL | 'A', 'B' |
| seat_number | INT | NOT NULL | 1, 2, 3 |
| seat_label | VARCHAR(50) | NOT NULL | Display: 'A-01' |
| seat_type | VARCHAR(50) | NOT NULL | 'standard', 'vip', 'accessible' |
| accessible | BOOLEAN | DEFAULT false | Wheelchair accessible |
| created_by | BIGINT | FK to sys_user | |
| created_at | TIMESTAMP | DEFAULT now() | |
| updated_by | BIGINT | FK to sys_user | |
| updated_at | TIMESTAMP | NULLABLE | |
| is_deleted | BOOLEAN | DEFAULT false | |

**Indexes**:
- `section_id, (section_id, row_label, seat_number)`

---

### 3. Event Tables

#### `event`
Events to sell tickets for

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| event_id | BIGSERIAL | PK | |
| event_code | VARCHAR(50) | UNIQUE, NOT NULL | 'EVT001' |
| event_name | VARCHAR(255) | NOT NULL | |
| description | TEXT | NULLABLE | |
| venue_id | BIGINT | FK to venue | |
| banner_url | VARCHAR(255) | NULLABLE | Event poster |
| start_time | TIMESTAMP | NOT NULL (without tz) | Event start datetime |
| end_time | TIMESTAMP | NOT NULL (without tz) | Event end datetime |
| sale_start_time | TIMESTAMP | NOT NULL (without tz) | When tickets start selling |
| sale_end_time | TIMESTAMP | NOT NULL (without tz) | When tickets stop selling |
| status | VARCHAR(20) | NOT NULL | 'draft', 'published', 'on_sale', 'sale_closed', 'ended', 'cancelled' |
| published_at | TIMESTAMP | NULLABLE | When published |
| on_sale_at | TIMESTAMP | NULLABLE | When went on sale |
| is_featured | BOOLEAN | DEFAULT false | Display in featured section |
| is_trending | BOOLEAN | DEFAULT false | Display in trending section |
| display_order | INT | DEFAULT 0 | Sort order |
| created_by | BIGINT | FK to sys_user | |
| created_at | TIMESTAMP | DEFAULT now() | |
| updated_by | BIGINT | FK to sys_user | |
| updated_at | TIMESTAMP | NULLABLE | |
| is_deleted | BOOLEAN | DEFAULT false | |

**Indexes**:
- `event_id, status, is_featured, is_trending`
- `(status, start_time DESC)` for upcoming

#### `event_zone`
Pricing zones within an event (Floor, VIP, Premium)

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| event_zone_id | BIGSERIAL | PK | |
| event_id | BIGINT | FK to event | |
| zone_code | VARCHAR(50) | NOT NULL | 'FLOOR', 'VIP' |
| zone_name | VARCHAR(100) | NOT NULL | |
| base_price | NUMERIC(18,2) | NOT NULL | Starting price |
| capacity | INT | NOT NULL | Seats in this zone |
| status | VARCHAR(20) | NOT NULL | 'active', 'inactive' |
| created_by | BIGINT | FK to sys_user | |
| created_at | TIMESTAMP | DEFAULT now() | |
| updated_by | BIGINT | FK to sys_user | |
| updated_at | TIMESTAMP | NULLABLE | |
| is_deleted | BOOLEAN | DEFAULT false | |

**Indexes**:
- `event_id, event_zone_id`

#### `event_zone_price`
Time-based pricing tiers per zone

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| event_zone_price_id | BIGSERIAL | PK | |
| event_zone_id | BIGINT | FK to event_zone | |
| price | NUMERIC(18,2) | NOT NULL | Ticket price for this tier |
| currency | VARCHAR(10) | DEFAULT 'VND' | |
| start_time | TIMESTAMP | NULLABLE (without tz) | When this price starts (early bird, etc.) |
| end_time | TIMESTAMP | NULLABLE (without tz) | When this price ends |
| is_active | BOOLEAN | DEFAULT true | |
| created_by | BIGINT | FK to sys_user | |
| created_at | TIMESTAMP | DEFAULT now() | |
| updated_by | BIGINT | FK to sys_user | |
| updated_at | TIMESTAMP | NULLABLE | |
| is_deleted | BOOLEAN | DEFAULT false | |

**Indexes**:
- `event_zone_id, (event_zone_id, is_active)`

#### `event_zone_section`
Maps event zones to physical venue sections

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| event_zone_section_id | BIGSERIAL | PK | |
| event_id | BIGINT | FK to event | |
| event_zone_id | BIGINT | FK to event_zone | |
| section_id | BIGINT | FK to venue_section | |
| created_by | BIGINT | FK to sys_user | |
| created_at | TIMESTAMP | DEFAULT now() | |
| updated_by | BIGINT | FK to sys_user | |
| updated_at | TIMESTAMP | NULLABLE | |
| is_deleted | BOOLEAN | DEFAULT false | |

**Unique Constraint**:
- `UNIQUE (event_id, event_zone_id, section_id) WHERE is_deleted=false`

**Indexes**:
- `(event_id, event_zone_id, section_id)`

#### `event_seat_inventory`
Real-time inventory per seat per event

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| event_seat_inventory_id | BIGSERIAL | PK | |
| event_id | BIGINT | FK to event | |
| venue_seat_id | BIGINT | FK to venue_seat | |
| status | VARCHAR(20) | NOT NULL | 'available', 'held', 'sold', 'blocked' |
| current_hold_id | BIGINT | NULLABLE | FK to seat_hold (if held) |
| order_item_id | BIGINT | NULLABLE | FK to ticket_order_item (if sold) |
| version | INT | DEFAULT 1 | Optimistic lock version |
| created_at | TIMESTAMP | DEFAULT now() | |
| updated_at | TIMESTAMP | NULLABLE | |

**Indexes**:
- `event_id, (event_id, status)`
- `(status, updated_at)` for cleanup jobs

**Constraint**: Only one order_item_id per seat (prevents double-sell)

---

### 4. Seat Hold (Reservation) Tables

#### `seat_hold`
Temporary holds before checkout

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| hold_id | BIGSERIAL | PK | |
| hold_code | VARCHAR(50) | UNIQUE, NOT NULL | 'HOLD-UUID' |
| event_id | BIGINT | FK to event | |
| user_id | BIGINT | FK to sys_user | |
| status | VARCHAR(20) | NOT NULL | 'active', 'expired', 'released', 'converted', 'cancelled' |
| hold_started_at | TIMESTAMP | NOT NULL (without tz) | |
| hold_expires_at | TIMESTAMP | NOT NULL (without tz) | Usually now() + 10 minutes |
| released_at | TIMESTAMP | NULLABLE (without tz) | When manually released |
| release_reason | VARCHAR(100) | NULLABLE | 'user_request', 'payment_failed', 'auto_expire' |
| created_at | TIMESTAMP | DEFAULT now() | |

**Indexes**:
- `(event_id, status, hold_expires_at)` for expiry jobs
- `(user_id, event_id, status)` for user's holds

**Check Constraint**: `hold_expires_at > hold_started_at`

#### `seat_hold_item`
Individual seat within a hold

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| hold_item_id | BIGSERIAL | PK | |
| hold_id | BIGINT | FK to seat_hold | |
| event_seat_inventory_id | BIGINT | FK to event_seat_inventory | |
| seat_id | BIGINT | FK to venue_seat | |
| zone_id | BIGINT | FK to event_zone | |
| price_at_hold | NUMERIC(18,2) | NOT NULL | Price locked at hold time |
| seat_label_snapshot | VARCHAR(50) | NULLABLE | 'A-01' (for history) |
| zone_name_snapshot | VARCHAR(100) | NULLABLE | 'VIP' (for history) |
| status | VARCHAR(20) | NOT NULL | 'active', 'released', 'converted', 'expired' |
| created_at | TIMESTAMP | DEFAULT now() | |

**Unique Constraint**: `UNIQUE (hold_id, event_seat_inventory_id)`

**Indexes**:
- `hold_id, (hold_id, status)`

---

### 5. Order & Ticket Tables

#### `ticket_order`
Checkout cart / order

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| order_id | BIGSERIAL | PK | |
| order_code | VARCHAR(50) | UNIQUE, NOT NULL | 'ORD-{guid}' |
| event_id | BIGINT | FK to event | |
| user_id | BIGINT | FK to sys_user | |
| hold_id | BIGINT | NULLABLE | FK to seat_hold (source hold) |
| total_price | NUMERIC(18,2) | NOT NULL | Sum of order items |
| currency | VARCHAR(10) | DEFAULT 'VND' | |
| status | VARCHAR(20) | NOT NULL | 'pending', 'confirmed', 'cancelled', 'refunded' |
| payment_deadline | TIMESTAMP | NOT NULL (without tz) | When order expires if unpaid |
| confirmed_at | TIMESTAMP | NULLABLE (without tz) | When payment confirmed |
| created_at | TIMESTAMP | DEFAULT now() | |
| updated_at | TIMESTAMP | NULLABLE | |

**Indexes**:
- `user_id, event_id`
- `(status, created_at)` for unsettled orders

#### `ticket_order_item`
Individual seat in an order

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| order_item_id | BIGSERIAL | PK | |
| order_id | BIGINT | FK to ticket_order | |
| event_seat_inventory_id | BIGINT | FK to event_seat_inventory | |
| seat_id | BIGINT | FK to venue_seat | |
| zone_id | BIGINT | FK to event_zone | |
| price | NUMERIC(18,2) | NOT NULL | Price for this seat |
| created_at | TIMESTAMP | DEFAULT now() | |

**Constraint**: Only one order_item_id per event_seat_inventory

#### `ticket`
Issued ticket after payment confirmed

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| ticket_id | BIGSERIAL | PK | |
| ticket_code | VARCHAR(50) | UNIQUE, NOT NULL | QR code / barcode |
| order_item_id | BIGINT | FK to ticket_order_item | |
| event_id | BIGINT | FK to event | |
| venue_seat_id | BIGINT | FK to venue_seat | |
| user_id | BIGINT | FK to sys_user | |
| status | VARCHAR(20) | NOT NULL | 'issued', 'used', 'cancelled', 'refunded' |
| issued_at | TIMESTAMP | DEFAULT now() | |
| used_at | TIMESTAMP | NULLABLE | When scanned at entry |
| cancelled_at | TIMESTAMP | NULLABLE | |

**Indexes**:
- `ticket_code`, `event_id, user_id`

---

### 6. Payment Tables

#### `payment_transaction`
Payment records

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| payment_id | BIGSERIAL | PK | |
| order_id | BIGINT | FK to ticket_order | |
| payment_provider | VARCHAR(30) | NOT NULL | 'vnpay', 'momo', 'mock' |
| payment_ref | VARCHAR(100) | UNIQUE, NOT NULL | Internal reference |
| provider_transaction_ref | VARCHAR(100) | UNIQUE, NULLABLE | External transaction ID |
| amount | NUMERIC(18,2) | NOT NULL | Amount to pay |
| payment_status | VARCHAR(20) | NOT NULL | 'initiated', 'pending', 'success', 'failed', 'cancelled' |
| requested_at | TIMESTAMP | NOT NULL (without tz) | When payment requested |
| confirmed_at | TIMESTAMP | NULLABLE (without tz) | When confirmed by provider |
| raw_request_payload | TEXT | NULLABLE | Request JSON |
| raw_callback_payload | TEXT | NULLABLE | Callback JSON |
| created_at | TIMESTAMP | DEFAULT now() | |
| updated_at | TIMESTAMP | NULLABLE | |

**Indexes**:
- `(order_id, payment_status)`
- `provider_transaction_ref`

#### `payment_callback_log`
Audit trail of payment callbacks

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| callback_log_id | BIGSERIAL | PK | |
| payment_id | BIGINT | NULLABLE | FK to payment_transaction |
| payment_provider | VARCHAR(30) | NOT NULL | |
| external_transaction_ref | VARCHAR(100) | NULLABLE | |
| callback_signature | VARCHAR(500) | NOT NULL | Signature for validation |
| payload | TEXT | NOT NULL | Raw callback body |
| signature_valid | BOOLEAN | NOT NULL | Whether signature verified |
| processed_status | VARCHAR(20) | NOT NULL | 'received', 'processed', 'ignored', 'failed' |
| received_at | TIMESTAMP | DEFAULT now() | |
| processed_at | TIMESTAMP | NULLABLE | |

**Indexes**:
- `(payment_id, processed_status)`
- `(external_transaction_ref, payment_provider)`

---

### 7. Idempotency & Audit Tables

#### `idempotency_request`
Prevent duplicate operations

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| idempotency_id | BIGSERIAL | PK | |
| idempotency_key | VARCHAR(100) | NOT NULL | Client-provided unique ID |
| request_type | VARCHAR(50) | NOT NULL | 'hold_seats', 'checkout', 'payment_callback' |
| user_id | BIGINT | NULLABLE | FK to sys_user |
| request_hash | VARCHAR(200) | NULLABLE | Hash of request body (detect mutations) |
| status | VARCHAR(20) | NOT NULL | 'processing', 'completed', 'failed', 'expired' |
| response_snapshot | TEXT | NULLABLE | Response JSON if completed |
| created_at | TIMESTAMP | DEFAULT now() | |
| expired_at | TIMESTAMP | NULLABLE | Auto-cleanup after this time |

**Unique Constraint**: `UNIQUE (request_type, idempotency_key)`

#### `event_publish_log`
Track event status transitions

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| event_publish_log_id | BIGSERIAL | PK | |
| event_id | BIGINT | FK to event | |
| action | VARCHAR(30) | NOT NULL | 'publish', 'open_sale', 'close_sale', 'cancel' |
| old_status | VARCHAR(30) | NULLABLE | Status before change |
| new_status | VARCHAR(30) | NOT NULL | Status after change |
| changed_by | BIGINT | NULLABLE | FK to sys_user |
| changed_at | TIMESTAMP | DEFAULT now() | |
| note | VARCHAR(1000) | NULLABLE | Why changed (admin notes) |

**Indexes**:
- `(event_id, changed_at DESC)`

#### `audit_log`
Full audit trail of all changes

| Column | Type | Constraints | Purpose |
|--------|------|-------------|---------|
| audit_log_id | BIGSERIAL | PK | |
| actor_user_id | BIGINT | NULLABLE | FK to sys_user (who made change) |
| module_name | VARCHAR(50) | NOT NULL | 'Event', 'Venue', 'User' |
| action_name | VARCHAR(50) | NOT NULL | 'INSERT', 'UPDATE', 'DELETE' |
| entity_name | VARCHAR(50) | NOT NULL | 'Event', 'EventZone' |
| entity_id | BIGINT | NULLABLE | ID of changed entity |
| old_data | TEXT | NULLABLE | JSON before (for UPDATE) |
| new_data | TEXT | NULLABLE | JSON after (for INSERT/UPDATE) |
| ip_address | VARCHAR(50) | NULLABLE | Request IP |
| created_at | TIMESTAMP | DEFAULT now() | |

**Indexes**:
- `(entity_name, entity_id, created_at DESC)`
- `(actor_user_id, created_at DESC)`
- `(module_name, created_at DESC)`

---

## Critical Indexes

| Table | Columns | Reason |
|-------|---------|--------|
| `event_seat_inventory` | `(event_id, status)` | Real-time inventory queries |
| `event` | `(status, start_time DESC)` | Upcoming events list |
| `seat_hold` | `(event_id, status, hold_expires_at)` | Expiry cleanup jobs |
| `ticket_order` | `(user_id, created_at DESC)` | User's orders |
| `ticket` | `(event_id, user_id)` | User's tickets for event |
| `audit_log` | `(entity_name, entity_id, created_at DESC)` | Change history lookup |
| `event_zone_price` | `(event_zone_id, is_active)` | Pricing queries |

---

## Data Types & Conventions

| Type | Usage | Notes |
|------|-------|-------|
| `BIGSERIAL` | Primary keys | Auto-increment long for high-volume tables |
| `NUMERIC(18,2)` | Money | Always use for financial amounts (avoid FLOAT) |
| `VARCHAR(n)` | Text | Use appropriate length constraints |
| `TEXT` | Long text | JSON payloads, descriptions |
| `TIMESTAMP without time zone` | All datetimes | Consistency across timezones, in UTC always |
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


