# 📢 Event API Update - v1.1

**Release Date**: April 1, 2026  
**Status**: ✅ LIVE

---

## 🎯 Summary

Event API đã được cập nhật thêm **3 field mới** để hỗ trợ tính năng quản lý sự kiện nổi bật và xu hướng.

---

## ✨ New Fields

| Field | Type | Default | Description |
|-------|------|---------|-------------|
| `is_featured` | boolean | false | Đánh dấu sự kiện nổi bật |
| `is_trending` | boolean | false | Đánh dấu sự kiện xu hướng |
| `display_order` | integer | 0 | Thứ tự hiển thị (≥ 0) |

---

## 📡 Affected Endpoints

### POST `/api/admin/event/insert`
**Thêm 3 fields vào request body:**
```json
{
  "is_featured": true,
  "is_trending": false,
  "display_order": 1
}
```

### PUT `/api/admin/event/update`
**Thêm 3 fields vào request body:**
```json
{
  "event_id": 1,
  "is_featured": true,
  "is_trending": false,
  "display_order": 1
}
```

### GET `/api/admin/event/getbyid`
**Response bao gồm 3 fields mới**

### GET `/api/admin/event/getpagedlist`
**Thêm 2 query parameters (optional):**
```
?is_featured=true&is_trending=false
```

### DELETE `/api/admin/event/delete`
**Không có thay đổi**

---

## 📝 Complete Example

### Create Event Request:
```json
{
  "event_code": "EVT001",
  "event_name": "Sky Tour Concert 2026",
  "description": "Live concert event",
  "venue_id": 1,
  "banner_url": "https://example.com/banner.jpg",
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
```

### Response:
```json
{
  "issuccess": true,
  "message": "Thêm mới sự kiện thành công",
  "data": 1,
  "statuscode": 200
}
```

---

## ✅ Frontend TODO

- [ ] Update TypeScript Event model - add 3 new fields
- [ ] Update EventCreateRequest/EventUpdateRequest DTOs
- [ ] Update Event Form - add 2 toggle switches + 1 number input
- [ ] Update Event List - add 2 new columns + filter UI
- [ ] Update API service - handle new parameters
- [ ] Test all CRUD operations
- [ ] Test filter with is_featured & is_trending

---

## 🔧 Query String Examples

### Get Featured Events Only:
```
GET /api/admin/event/getpagedlist?pagesize=20&offset=0&is_featured=true
```

### Get Trending Events Only:
```
GET /api/admin/event/getpagedlist?pagesize=20&offset=0&is_trending=true
```

### Get Both Featured AND Trending:
```
GET /api/admin/event/getpagedlist?pagesize=20&offset=0&is_featured=true&is_trending=true
```

### Get All Events (No Filter):
```
GET /api/admin/event/getpagedlist?pagesize=20&offset=0
```

---

## 📚 Data Types & Validation

### is_featured
- **Type**: `boolean`
- **Required**: Yes (Form)
- **Valid Values**: `true`, `false`
- **Default**: `false`

### is_trending
- **Type**: `boolean`
- **Required**: Yes (Form)
- **Valid Values**: `true`, `false`
- **Default**: `false`

### display_order
- **Type**: `integer`
- **Required**: Yes (Form)
- **Min Value**: `0`
- **Max Value**: `2147483647` (int32 max)
- **Default**: `0`
- **Error Message**: "display_order phải lớn hơn hoặc bằng 0"

---

## 🌐 API Documentation

**Swagger Endpoint**: `http://localhost:5025/swagger`

Test all endpoints directly from Swagger UI:
1. Navigate to `/swagger`
2. Find Event section
3. Try out all endpoints with new fields

---

## 💻 Implementation Checklist

### Backend ✅
- [x] Add 3 fields to Event table
- [x] Update `event_insert()` function
- [x] Update `event_update()` function
- [x] Update `event_getbyid()` function
- [x] Update `event_getpagedlist()` function
- [x] Add filters to Entity/DTO
- [x] Update Domain Service
- [x] Update Use Cases
- [x] Update API Endpoints

### Frontend ⏳
- [ ] Update TypeScript models
- [ ] Update DTOs
- [ ] Update Components
- [ ] Update Services
- [ ] Test & Deploy

---

## ❓ FAQ

**Q: Do I need to update old Events?**  
A: No. New fields will default to `false` (is_featured, is_trending) and `0` (display_order).

**Q: Can I filter by multiple conditions?**  
A: Yes. Use query params: `?status=on_sale&is_featured=true&is_trending=false`

**Q: Is display_order required?**  
A: Yes, but defaults to 0 if not provided during creation.

---

## 📞 Support

Contact Backend Team for any questions or issues.

**Last Updated**: April 1, 2026

