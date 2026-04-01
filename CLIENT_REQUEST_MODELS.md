# 📱 CLIENT REQUEST MODELS - Integration Guide

**Status**: ✅ READY  
**Date**: April 1, 2026

---

## 📋 Request Models Available

Tất cả 4 methods đều dùng **Request Models** thay vì raw parameters.

### 1. EventGetFeaturedClientRequest
```typescript
interface EventGetFeaturedClientRequest {
  limit: number;  // Default: 8
}
```

**Example:**
```javascript
const request = { limit: 8 };
const response = await fetch('/api/client/event/featured?limit=8');
```

---

### 2. EventGetTrendingClientRequest
```typescript
interface EventGetTrendingClientRequest {
  limit: number;  // Default: 12
}
```

**Example:**
```javascript
const request = { limit: 12 };
const response = await fetch('/api/client/event/trending?limit=12');
```

---

### 3. EventGetUpcomingClientRequest
```typescript
interface EventGetUpcomingClientRequest {
  limit: number;  // Default: 12
}
```

**Example:**
```javascript
const request = { limit: 12 };
const response = await fetch('/api/client/event/upcoming?limit=12');
```

---

### 4. EventSearchClientRequest
```typescript
interface EventSearchClientRequest {
  pagesize?: number;      // Default: 12
  offset?: number;        // Default: 0
  keysearch?: string;     // Optional
  status?: string;        // Optional: published, on_sale, ended
  venue_id?: number;      // Default: -1 (all venues)
  is_featured?: boolean;  // Optional
  is_trending?: boolean;  // Optional
  from_date?: string;     // Optional: ISO 8601 DateTime
  to_date?: string;       // Optional: ISO 8601 DateTime
}
```

**Example:**
```javascript
const request = {
  pagesize: 20,
  offset: 0,
  keysearch: 'concert',
  status: 'on_sale',
  venue_id: 1,
  is_featured: true,
  from_date: '2026-05-01T00:00:00Z',
  to_date: '2026-06-30T23:59:59Z'
};
const response = await fetch('/api/client/event/search', { 
  params: request 
});
```

---

## 🚀 React Integration

### Step 1: Create Types
```typescript
// src/types/event.ts
export interface EventGetFeaturedClientRequest {
  limit?: number;
}

export interface EventGetTrendingClientRequest {
  limit?: number;
}

export interface EventGetUpcomingClientRequest {
  limit?: number;
}

export interface EventSearchClientRequest {
  pagesize?: number;
  offset?: number;
  keysearch?: string;
  status?: string;
  venue_id?: number;
  is_featured?: boolean;
  is_trending?: boolean;
  from_date?: string;
  to_date?: string;
}

export interface EventClientDto {
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
  start_time: string;
  end_time: string;
  sale_start_time?: string;
  sale_end_time?: string;
  status: string;
  published_at?: string;
  on_sale_at?: string;
  is_featured: boolean;
  is_trending: boolean;
  display_order: number;
}
```

### Step 2: Create API Service
```typescript
// src/services/eventClientApi.ts
import axios from 'axios';
import {
  EventGetFeaturedClientRequest,
  EventGetTrendingClientRequest,
  EventGetUpcomingClientRequest,
  EventSearchClientRequest,
  EventClientDto,
} from '@/types/event';

const API_BASE = 'http://localhost:5025/api/client/event';

export const eventClientApi = {
  getFeatured: (request: EventGetFeaturedClientRequest) =>
    axios.get<{ data: EventClientDto[] }>(`${API_BASE}/featured`, {
      params: request,
    }),

  getTrending: (request: EventGetTrendingClientRequest) =>
    axios.get<{ data: EventClientDto[] }>(`${API_BASE}/trending`, {
      params: request,
    }),

  getUpcoming: (request: EventGetUpcomingClientRequest) =>
    axios.get<{ data: EventClientDto[] }>(`${API_BASE}/upcoming`, {
      params: request,
    }),

  search: (request: EventSearchClientRequest) =>
    axios.get<{ data: EventClientDto[] }>(`${API_BASE}/search`, {
      params: request,
    }),
};
```

### Step 3: Use in Components
```typescript
// src/pages/HomePage.tsx
import React, { useEffect, useState } from 'react';
import { eventClientApi } from '@/services/eventClientApi';
import { EventClientDto } from '@/types/event';

export const HomePage: React.FC = () => {
  const [featured, setFeatured] = useState<EventClientDto[]>([]);
  const [trending, setTrending] = useState<EventClientDto[]>([]);

  useEffect(() => {
    // Get featured events (default: 8)
    eventClientApi.getFeatured({}).then(res => {
      setFeatured(res.data.data);
    });

    // Get trending events (default: 12)
    eventClientApi.getTrending({}).then(res => {
      setTrending(res.data.data);
    });
  }, []);

  return (
    <div>
      <section>
        <h2>Featured</h2>
        {featured.map(event => (
          <div key={event.event_id}>
            <h3>{event.event_name}</h3>
            <p>{event.description}</p>
          </div>
        ))}
      </section>

      <section>
        <h2>Trending</h2>
        {trending.map(event => (
          <div key={event.event_id}>
            <h3>{event.event_name}</h3>
            <p>{event.description}</p>
          </div>
        ))}
      </section>
    </div>
  );
};
```

### Step 4: Search with Filters
```typescript
// src/pages/ExploreEvents.tsx
import React, { useState } from 'react';
import { eventClientApi } from '@/services/eventClientApi';
import { EventSearchClientRequest, EventClientDto } from '@/types/event';

export const ExploreEvents: React.FC = () => {
  const [results, setResults] = useState<EventClientDto[]>([]);
  const [filters, setFilters] = useState<EventSearchClientRequest>({
    pagesize: 20,
    offset: 0,
  });

  const handleSearch = async () => {
    const response = await eventClientApi.search(filters);
    setResults(response.data.data);
  };

  return (
    <div>
      {/* Filter UI */}
      <input
        placeholder="Search..."
        onChange={(e) =>
          setFilters({ ...filters, keysearch: e.target.value })
        }
      />

      <select
        onChange={(e) => setFilters({ ...filters, status: e.target.value })}
      >
        <option value="">All Status</option>
        <option value="published">Published</option>
        <option value="on_sale">On Sale</option>
        <option value="ended">Ended</option>
      </select>

      <button onClick={handleSearch}>Search</button>

      {/* Results */}
      {results.map(event => (
        <div key={event.event_id}>{event.event_name}</div>
      ))}
    </div>
  );
};
```

---

## 📊 Query Parameter Examples

### Featured (Limit 8)
```
GET /api/client/event/featured?limit=8
```

### Trending (Limit 12)
```
GET /api/client/event/trending?limit=12
```

### Upcoming (Limit 12)
```
GET /api/client/event/upcoming?limit=12
```

### Search All
```
GET /api/client/event/search?pagesize=20&offset=0
```

### Search with Filters
```
GET /api/client/event/search?pagesize=20&offset=0&keysearch=concert&status=on_sale&is_featured=true&from_date=2026-05-01T00:00:00Z&to_date=2026-06-30T23:59:59Z
```

### Search - Page 2
```
GET /api/client/event/search?pagesize=20&offset=20
```

---

## ✅ Key Points

✅ All request models have **sensible defaults**
✅ All parameters are **optional** except in their respective models
✅ Date parameters must be in **ISO 8601 format**
✅ `venue_id = -1` means **all venues**
✅ `null` values for boolean filters mean **no filter**

---

## 🧪 Test with Swagger

Visit: `http://localhost:5025/swagger`

Look for **EventClient** section and test each endpoint with request models!

---

## 📞 Questions?

Check documentation:
- `CLIENT_EVENT_API.md` - Full API spec
- `CLIENT_EVENTS_READY.md` - Quick reference

---

**Ready to integrate!** 🚀

