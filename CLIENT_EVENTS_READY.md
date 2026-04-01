# 📢 Client Event API - Ready for Integration

**Status**: ✅ DEPLOYED  
**Release Date**: April 1, 2026

---

## 🎯 Summary

Backend API cho Client-side đã sẵn sàng:

**3 New Endpoints:**
- ✅ `GET /api/client/event/featured` - Featured events for homepage hero
- ✅ `GET /api/client/event/trending` - Trending events section
- ✅ `GET /api/client/event/upcoming` - Upcoming events
- ✅ `GET /api/client/event/search` - Advanced search/filter

**Đặc điểm:**
- ✅ Public endpoints (no authentication)
- ✅ Lightweight DTOs (only essential fields)
- ✅ High-performance with DB indexes
- ✅ Pagination support
- ✅ Advanced filtering

---

## 📱 Endpoints Reference

### Featured Events
```
GET /api/client/event/featured?limit=8
```
**For**: Homepage hero carousel (8 events default)

### Trending Events
```
GET /api/client/event/trending?limit=12
```
**For**: Trending section (12 events default)

### Upcoming Events
```
GET /api/client/event/upcoming?limit=12
```
**For**: Upcoming events section (12 events default)

### Search/Filter
```
GET /api/client/event/search?pagesize=12&offset=0&keysearch=concert&is_featured=true
```
**For**: Explore/Discovery page with advanced filters

---

## 📊 Available Filters (Search Endpoint)

| Parameter | Type | Required | Notes |
|-----------|------|----------|-------|
| pagesize | int | No | Default: 12 |
| offset | int | No | Default: 0 |
| keysearch | string | No | Search by code/name/description/venue/city |
| status | string | No | published, on_sale, ended |
| venue_id | long | No | -1 = all venues |
| is_featured | boolean | No | Filter featured events |
| is_trending | boolean | No | Filter trending events |
| from_date | datetime | No | ISO 8601 format |
| to_date | datetime | No | ISO 8601 format |

---

## 📋 Response Structure

All endpoints return:
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

---

## 🚀 Quick Start Examples

### Homepage Hero (Featured)
```javascript
const featured = await fetch('/api/client/event/featured?limit=8')
  .then(r => r.json())
  .then(r => r.data);

// Use featured[0] as main hero
// Use featured[1-7] for carousel
```

### Homepage Trending
```javascript
const trending = await fetch('/api/client/event/trending?limit=12')
  .then(r => r.json())
  .then(r => r.data);

// Display in 3x4 grid
```

### Homepage Upcoming
```javascript
const upcoming = await fetch('/api/client/event/upcoming?limit=12')
  .then(r => r.json())
  .then(r => r.data);

// Display in carousel/list
```

### Explore Page (Advanced Search)
```javascript
const params = new URLSearchParams({
  pagesize: 20,
  offset: 0,
  keysearch: 'concert',
  status: 'on_sale',
  venue_id: 1,
  is_featured: true,
  from_date: '2026-05-01T00:00:00Z',
  to_date: '2026-06-30T23:59:59Z'
});

const results = await fetch(`/api/client/event/search?${params}`)
  .then(r => r.json())
  .then(r => r.data);
```

---

## 🔍 TypeScript Types (For React)

```typescript
interface EventClientDto {
  event_id: number;
  event_code: string;
  event_name: string;
  description?: string;
  venue_id: number;
  venue_code?: string;
  venue_name?: string;
  city?: string;
  country?: string;
  banner_url?: string;
  start_time: string;      // ISO 8601
  end_time: string;
  sale_start_time?: string;
  sale_end_time?: string;
  status: string;           // published | on_sale | ended
  published_at?: string;
  on_sale_at?: string;
  is_featured: boolean;    // 🆕 Featured flag
  is_trending: boolean;    // 🆕 Trending flag
  display_order: number;   // 🆕 Display order
}

interface ApiResponse<T> {
  issuccess: boolean;
  message: string;
  data: T;
  statuscode: number;
}
```

---

## 📖 Full Documentation

See detailed API docs: `CLIENT_EVENT_API.md`

---

## ✅ Implementation Checklist (Frontend)

- [ ] Add `EventClientDto` TypeScript interface
- [ ] Create API service functions for 4 endpoints
- [ ] Build Homepage Featured section (8 events carousel)
- [ ] Build Homepage Trending section (12 events grid)
- [ ] Build Homepage Upcoming section (12 events carousel)
- [ ] Build Explore page with search + filters
- [ ] Test all endpoints
- [ ] Add pagination handling
- [ ] Add loading/error states
- [ ] Integrate with routing

---

## 🧪 Testing (Swagger)

Navigate to: `http://localhost:5025/swagger`

Find and test:
1. `/api/client/event/featured`
2. `/api/client/event/trending`
3. `/api/client/event/upcoming`
4. `/api/client/event/search`

All endpoints are **public** (no auth needed)

---

## 🏗️ Architecture

```
EventClientController
    ↓
EventClientUseCases (Application Layer)
    ↓
IEventDomainService.GetFeaturedAsync()
    ↓
IEventRepository.GetFeaturedAsync()
    ↓
event_get_featured_client() [PostgreSQL Function]
    ↓
event table + venue join
```

**Performance Optimizations:**
- ✅ Indexed queries (3 main indexes)
- ✅ No COUNT(*) OVER() - fast pagination
- ✅ No sys_user joins - lightweight
- ✅ Minimal fields - low bandwidth
- ✅ Refcursor + LIMIT optimization

---

## 📞 Support

Questions? Contact Backend Team or check:
- Swagger UI: `http://localhost:5025/swagger`
- GitHub: `CLIENT_EVENT_API.md`
- Slack: #backend-team

---

**All Set! Ready for client integration.** 🚀

Last Updated: April 1, 2026

