# 📱 Client Event API - v1.0

**Status**: ✅ READY  
**Date**: April 1, 2026

---

## 🎯 Overview

Client Event APIs dùng cho **Homepage**, **Explore Page**, **Discovery** page...

**Đặc điểm:**
- ✅ Không yêu cầu authentication
- ✅ Lightweight DTOs (chỉ fields cần thiết)
- ✅ Tối ưu performance với DB indexes
- ✅ Public endpoints

---

## 📡 Base URL

```
http://localhost:5025/api/client/event
```

---

## 🔗 Endpoints

### 1. Get Featured Events
```
GET /api/client/event/featured?limit=8
```

**Description**: Lấy 8 sự kiện nổi bật cho Homepage hero section

**Query Parameters**:
| Name | Type | Default | Description |
|------|------|---------|-------------|
| limit | integer | 8 | Số sự kiện cần lấy |

**Response**:
```json
{
  "issuccess": true,
  "message": "Lấy danh sách sự kiện nổi bật thành công",
  "data": [
    {
      "event_id": 1,
      "event_code": "EVT001",
      "event_name": "Sky Tour Concert 2026",
      "description": "Live concert",
      "venue_id": 1,
      "venue_code": "VEN001",
      "venue_name": "SVD Center",
      "city": "Ho Chi Minh",
      "country": "Vietnam",
      "banner_url": "https://...",
      "start_time": "2026-06-20T19:30:00Z",
      "end_time": "2026-06-20T22:30:00Z",
      "sale_start_time": "2026-05-15T10:00:00Z",
      "sale_end_time": "2026-06-20T18:30:00Z",
      "status": "on_sale",
      "published_at": "2026-05-10T09:00:00Z",
      "on_sale_at": "2026-05-15T10:00:00Z",
      "is_featured": true,
      "is_trending": false,
      "display_order": 1
    }
  ],
  "statuscode": 200
}
```

**Sort Order**: `display_order ASC → start_time ASC → event_id DESC`

---

### 2. Get Trending Events
```
GET /api/client/event/trending?limit=12
```

**Description**: Lấy 12 sự kiện xu hướng cho "Trending" section

**Query Parameters**:
| Name | Type | Default | Description |
|------|------|---------|-------------|
| limit | integer | 12 | Số sự kiện cần lấy |

**Response**: Same as Featured (see above)

**Sort Order**: `is_featured DESC → display_order ASC → start_time ASC → event_id DESC`

---

### 3. Get Upcoming Events
```
GET /api/client/event/upcoming?limit=12
```

**Description**: Lấy 12 sự kiện sắp tới (start_time >= now)

**Query Parameters**:
| Name | Type | Default | Description |
|------|------|---------|-------------|
| limit | integer | 12 | Số sự kiện cần lấy |

**Response**: Same as Featured (see above)

**Sort Order**: `is_featured DESC → is_trending DESC → display_order ASC → start_time ASC → event_id DESC`

---

### 4. Search Events
```
GET /api/client/event/search?pagesize=12&offset=0&keysearch=concert&is_featured=true
```

**Description**: Advanced search/filter cho Explore page

**Query Parameters**:
| Name | Type | Default | Description |
|------|------|---------|-------------|
| pagesize | integer | 12 | Số bản ghi một trang |
| offset | integer | 0 | Offset cho phân trang |
| keysearch | string | - | Tìm kiếm theo: event_code, event_name, description, venue_name, city |
| status | string | - | Lọc trạng thái: `published`, `on_sale`, `ended` |
| venue_id | long | -1 | Lọc theo venue ID (-1 = tất cả) |
| is_featured | boolean | - | Lọc sự kiện nổi bật (true/false) |
| is_trending | boolean | - | Lọc sự kiện xu hướng (true/false) |
| from_date | datetime | - | Lọc sự kiện từ ngày (ISO 8601) |
| to_date | datetime | - | Lọc sự kiện đến ngày (ISO 8601) |

**Response**:
```json
{
  "issuccess": true,
  "message": "Tìm kiếm sự kiện thành công",
  "data": [
    {
      "event_id": 1,
      ...event fields...
    }
  ],
  "statuscode": 200
}
```

**Query Examples**:
```
# Only featured events
GET /api/client/event/search?is_featured=true

# Trending + Upcoming events
GET /api/client/event/search?is_trending=true&from_date=2026-04-01T00:00:00Z

# Search + Filter by date range
GET /api/client/event/search?keysearch=concert&from_date=2026-05-01T00:00:00Z&to_date=2026-06-30T23:59:59Z

# By venue
GET /api/client/event/search?venue_id=1&status=on_sale
```

---

## 📊 Response Format

All responses follow this structure:

```typescript
interface ApiResponse<T> {
  issuccess: boolean;      // true/false
  message: string;          // Success/error message
  data: T;                 // Response data (array of events)
  statuscode: number;      // HTTP status code
}
```

---

## 🎯 Use Cases

### Homepage Hero Section
```javascript
// Get 8 featured events for hero carousel
const response = await fetch('/api/client/event/featured?limit=8');
const { data: featuredEvents } = await response.json();
// Use featuredEvents[0-7] for carousel
```

### Homepage Trending Section
```javascript
// Get 12 trending events
const response = await fetch('/api/client/event/trending?limit=12');
const { data: trendingEvents } = await response.json();
// Display in grid/carousel
```

### Homepage Upcoming Section
```javascript
// Get 12 upcoming events
const response = await fetch('/api/client/event/upcoming?limit=12');
const { data: upcomingEvents } = await response.json();
// Display in list/grid
```

### Explore/Discovery Page
```javascript
// Search with filters
const params = new URLSearchParams({
  pagesize: 20,
  offset: 0,
  keysearch: 'concert',
  venue_id: 1,
  is_featured: true,
  from_date: '2026-05-01T00:00:00Z',
  to_date: '2026-06-30T23:59:59Z'
});

const response = await fetch(`/api/client/event/search?${params}`);
const { data: events } = await response.json();
```

---

## 📋 Event DTO Fields

```typescript
interface EventClientDto {
  event_id: number;
  event_code: string;
  event_name: string;
  description?: string;
  
  // Venue Info
  venue_id: number;
  venue_code?: string;
  venue_name?: string;
  city?: string;
  country?: string;
  
  // Event Details
  banner_url?: string;
  start_time: string;          // ISO 8601 DateTime
  end_time: string;
  sale_start_time?: string;
  sale_end_time?: string;
  status: string;              // 'published' | 'on_sale' | 'ended'
  published_at?: string;
  on_sale_at?: string;
  
  // New Fields
  is_featured: boolean;        // Sự kiện nổi bật
  is_trending: boolean;        // Sự kiện xu hướng
  display_order: number;       // Thứ tự hiển thị (0+)
}
```

---

## 🚀 Performance

### Optimized Indexes
Database đã được tối ưu với 3 indexes chính:
- `ix_event_client_homepage_featured` - Featured events lookup
- `ix_event_client_homepage_trending` - Trending events lookup
- `ix_event_client_homepage_upcoming` - Upcoming events lookup
- `ix_venue_client_basic_lookup` - Venue join lookup

### Query Characteristics
- ✅ No COUNT(*) OVER() = Fast pagination
- ✅ No sys_user joins = Lightweight
- ✅ Minimal fields = Low bandwidth
- ✅ Refcursor + LIMIT = Optimized for client fetch

---

## ⚡ Pagination

For `search` endpoint with `pagesize=20`:

**Page 1**: `offset=0`
**Page 2**: `offset=20`
**Page 3**: `offset=40`

```javascript
// Calculate offset
offset = (pageNumber - 1) * pagesize;
```

---

## 🔍 Filter Logic

### Date Range Filter
- `from_date`: Events where `end_time >= from_date`
- `to_date`: Events where `start_time <= to_date`
- Both optional

### Status Filter
- `published`: Event đã publish
- `on_sale`: Event đang bán vé
- `ended`: Event đã kết thúc

### Featured/Trending
- Both are boolean flags
- Can be combined: `is_featured=true&is_trending=true`

---

## 📞 Support

**For issues or questions:**
- Contact Backend Team
- Check Swagger: `http://localhost:5025/swagger`
- Test endpoints directly in Swagger UI

---

## 📌 Notes

- ✅ All timestamps are in UTC (ISO 8601)
- ✅ No authentication required
- ✅ No rate limiting (yet)
- ✅ CORS enabled for client domain
- ✅ All responses include `issuccess` flag for easy error handling

---

**Last Updated**: April 1, 2026

