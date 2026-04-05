using Ticketing.Application.Model.DTOs;
using Ticketing.Domain.Constants;
using Ticketing.Domain.Domain.Event.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Admin.Event.Request;
using Ticketing.Infrastructure.DTOs.Admin.EventSeatInventory.Response;
using Ticketing.Infrastructure.DTOs.Admin.EventZonePrice.Response;
using Ticketing.Infrastructure.DTOs.Client.Event.Request;
using Ticketing.Infrastructure.DTOs.Client.Event.Response;
using Ticketing.Infrastructure.DTOs.Event.Request;
using Ticketing.Infrastructure.DTOs.Event.Response;
using Ticketing.Infrastructure.DTOs.EventZonePrice.Response;
using Ticketing.Infrastructure.Entities;
using Ticketing.Infrastructure.Entities.Event.Response;
using Ticketing.Infrastructure.Entities.EventZoneSection.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Helpers.Interfaces;
using Ticketing.Infrastructure.Repositories.Event;
using Ticketing.Infrastructure.Repositories.Venue;

namespace Ticketing.Domain.Domain.Event;

public class EventDomainService(
    IEventUnitOfWork unitOfWork,
    IVenueUnitOfWork venueUnitOfWork,
    ICacheService cacheService,
    ICacheKeyHelper cacheKeyHelper)
    : BaseService<IEventRepository, EventEntity>(unitOfWork.Event, TicketingTypeEnum.Event), IEventDomainService
{
    private static readonly TimeSpan EventDetailCacheTtl = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan EventSeatCache = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan EventClientListCacheTtl = TimeSpan.FromMinutes(3);
    private static readonly TimeSpan EventSearchCacheTtl = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan EventClientCacheVersionTtl = TimeSpan.FromDays(30);

    private const string EventClientCacheVersionKey = "ticketing:event:client:version";

    public async Task<ResponseMessage<int>> InsertAsync(EventEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.InsertAsync(new
            {
                event_code = entity.event_code,
                event_name = entity.event_name,
                description = entity.description,
                venue_id = entity.venue_id,
                banner_url = entity.banner_url,
                start_time = entity.start_time,
                end_time = entity.end_time,
                sale_start_time = entity.sale_start_time,
                sale_end_time = entity.sale_end_time,
                status = entity.status,
                published_at = entity.published_at,
                on_sale_at = entity.on_sale_at,
                is_featured = entity.is_featured,
                is_trending = entity.is_trending,
                display_order = entity.display_order,
                created_by = entity.created_by
            }, cancellationToken)!.ToIntAsync();

            if (result is not > 0)
                throw new Exception(DomainMessageConstants.Event.InsertError);

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            await BumpEventClientCacheVersionAsync(cancellationToken);

            return new ResponseMessage<int>().MessageSuccess(result ?? 0, DomainMessageConstants.Event.InsertSuccess);
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<int>().MessageError(e.Message);
        }
    }


    public async Task<ResponseMessage<bool>> UpdateAsync(EventEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.UpdateAsync(new
            {
                event_id = entity.event_id,
                event_code = entity.event_code,
                event_name = entity.event_name,
                description = entity.description,
                venue_id = entity.venue_id,
                banner_url = entity.banner_url,
                start_time = entity.start_time,
                end_time = entity.end_time,
                sale_start_time = entity.sale_start_time,
                sale_end_time = entity.sale_end_time,
                status = entity.status,
                published_at = entity.published_at,
                on_sale_at = entity.on_sale_at,
                is_featured = entity.is_featured,
                is_trending = entity.is_trending,
                display_order = entity.display_order,
                updated_by = entity.updated_by
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception(DomainMessageConstants.Event.UpdateError);

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            await RemoveEventDetailCacheAsync(entity.event_id, cancellationToken);
            await BumpEventClientCacheVersionAsync(cancellationToken);

            return new ResponseMessage<bool>().MessageSuccess(true, DomainMessageConstants.Event.UpdateSuccess);
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(EventEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.DeleteAsync(new
            {
                event_id = entity.event_id,
                updated_by = entity.updated_by
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception(DomainMessageConstants.Event.DeleteError);

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            await RemoveEventDetailCacheAsync(entity.event_id, cancellationToken);
            await BumpEventClientCacheVersionAsync(cancellationToken);

            return new ResponseMessage<bool>().MessageSuccess(true, DomainMessageConstants.Event.DeleteSuccess);
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<EventDetailDto?>> GetByIdAsync(EventGetByIdRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetAsync<EventDetailDto>(new
            {
                event_id = request.event_id
            }, cancellationToken);

            if (result is null)
                return new ResponseMessage<EventDetailDto?>().MessageWarning(DomainMessageConstants.Event.NotFound);

            return new ResponseMessage<EventDetailDto?>().MessageSuccess(result, DomainMessageConstants.Event.GetDetailSuccess);
        }
        catch (Exception e)
        {
            return new ResponseMessage<EventDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<EventListDto>>> GetPagedListAsync(EventGetPagedListRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetPagedAsync<EventListDto>(new
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                status = request.status,
                venue_id = request.venue_id,
                is_featured = request.is_featured,
                is_trending = request.is_trending
            }, cancellationToken);

            return new ResponseMessage<IEnumerable<EventListDto>>().MessageSuccess(result ?? [], DomainMessageConstants.Event.GetListSuccess);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventListDto>>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<EventClientListDto>>> GetFeaturedAsync(EventGetFeaturedClientRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheVersion = await GetEventClientCacheVersionAsync(cancellationToken);
            var cacheKey = cacheKeyHelper.Build("ticketing:event:featured:v1", new { cacheVersion, request });

            var cached = await cacheService.GetAsync<List<EventClientListDto>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return new ResponseMessage<IEnumerable<EventClientListDto>>().MessageSuccess(cached, "Lấy danh sách sự kiện nổi bật thành công");
            }

            var result = (await _repository.GetFeaturedAsync(request, cancellationToken)).ToList();
            await cacheService.SetAsync(cacheKey, result, EventClientListCacheTtl, cancellationToken);

            return new ResponseMessage<IEnumerable<EventClientListDto>>().MessageSuccess(result, "Lấy danh sách sự kiện nổi bật thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventClientListDto>>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<EventClientListDto>>> GetTrendingAsync(EventGetTrendingClientRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheVersion = await GetEventClientCacheVersionAsync(cancellationToken);
            var cacheKey = cacheKeyHelper.Build("ticketing:event:trending:v1", new { cacheVersion, request });

            var cached = await cacheService.GetAsync<List<EventClientListDto>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return new ResponseMessage<IEnumerable<EventClientListDto>>().MessageSuccess(cached, "Lấy danh sách sự kiện xu hướng thành công");
            }

            var result = (await _repository.GetTrendingAsync(request, cancellationToken)).ToList();
            await cacheService.SetAsync(cacheKey, result, EventClientListCacheTtl, cancellationToken);

            return new ResponseMessage<IEnumerable<EventClientListDto>>().MessageSuccess(result, "Lấy danh sách sự kiện xu hướng thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventClientListDto>>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<EventClientListDto>>> GetUpcomingAsync(EventGetUpcomingClientRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheVersion = await GetEventClientCacheVersionAsync(cancellationToken);
            var cacheKey = cacheKeyHelper.Build("ticketing:event:upcoming:v1", new { cacheVersion, request });

            var cached = await cacheService.GetAsync<List<EventClientListDto>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return new ResponseMessage<IEnumerable<EventClientListDto>>().MessageSuccess(cached, "Lấy danh sách sự kiện sắp tới thành công");
            }

            var result = (await _repository.GetUpcomingAsync(request, cancellationToken)).ToList();
            await cacheService.SetAsync(cacheKey, result, EventClientListCacheTtl, cancellationToken);

            return new ResponseMessage<IEnumerable<EventClientListDto>>().MessageSuccess(result, "Lấy danh sách sự kiện sắp tới thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventClientListDto>>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<EventClientListDto>>> SearchAsync(EventSearchClientRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheVersion = await GetEventClientCacheVersionAsync(cancellationToken);
            var cacheKey = cacheKeyHelper.Build("ticketing:event:search:v1", new { cacheVersion, request });

            var cached = await cacheService.GetAsync<List<EventClientListDto>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return new ResponseMessage<IEnumerable<EventClientListDto>>().MessageSuccess(cached, "Tìm kiếm sự kiện thành công");
            }

            var result = (await _repository.SearchAsync(request, cancellationToken)).ToList();
            await cacheService.SetAsync(cacheKey, result, EventSearchCacheTtl, cancellationToken);

            return new ResponseMessage<IEnumerable<EventClientListDto>>().MessageSuccess(result, "Tìm kiếm sự kiện thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventClientListDto>>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<EventClientDetailDto?>> GetEventDetail(EventGetByIdRequest request, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var cacheKey = cacheKeyHelper.Build("ticketing:event:detail:v1", request);
            var cachedResult = await cacheService.GetAsync<EventClientDetailDto>(cacheKey, cancellationToken);

            if (cachedResult is not null)
            {
                // ✅ Cache hit - nhưng vẫn cần fetch real-time seat status
                await PopulateSeatStatusAsync(cachedResult, request.event_id, cancellationToken);
                return new ResponseMessage<EventClientDetailDto?>().MessageSuccess(cachedResult, DomainMessageConstants.Event.GetDetailSuccess);
            }

            var result = await _repository.GetAsync<EventClientDetailDto>(new
            {
                event_id = request.event_id
            }, cancellationToken);

            if (result is null)
                return new ResponseMessage<EventClientDetailDto?>().MessageError(DomainMessageConstants.Event.NotFound);

            // 🎯 STEP 1: Lấy tất cả zones của event
            var zones = (await unitOfWork.EventZone.GetByEventId<EventZoneDto>(new
            {
                event_id = request.event_id
            }, cancellationToken)).ToList();

            if (zones.Count == 0)
            {
                result.zones = zones;
                await cacheService.SetAsync(cacheKey, result, EventDetailCacheTtl, cancellationToken);
                return new ResponseMessage<EventClientDetailDto?>().MessageSuccess(result, DomainMessageConstants.Event.GetDetailSuccess);
            }

            // 🎯 STEP 2: Batch - Lấy tất cả prices của tất cả zones (1 query thay vì N)
            var zoneIds = zones.Select(z => z.event_zone_id).ToArray();
            var allPrices = (await unitOfWork.EventZonePrice.GetByZoneIds<EventZonePriceDto>(new
            {
                event_zone_ids = zoneIds
            }, cancellationToken)).ToList();

            // 🎯 STEP 3: Batch - Lấy tất cả zone sections (1 query thay vì N)
            var allZoneSections = (await unitOfWork.EventZoneSection.GetByEventId<EventZoneSectionEntity>(new
            {
                event_id = request.event_id
            }, cancellationToken)).ToList();

            if (allZoneSections.Count > 0)
            {
                // 🎯 STEP 4: Batch - Lấy tất cả seats từ tất cả sections (1 query thay vì M)
                var sectionIds = allZoneSections.Select(s => s.section_id).Distinct().ToArray();
                var allSeats = (await venueUnitOfWork.VenueSeat.GetBySectionIds<EventVenueSeatDto>(new
                {
                    section_ids = sectionIds
                }, cancellationToken)).ToList();

                // ⚡ Map in-memory (không gọi DB thêm nữa)
                var seatLookup = allSeats
                    .GroupBy(s => s.section_id)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var zoneSectionLookup = allZoneSections
                    .GroupBy(zs => zs.event_zone_id)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var priceLookup = allPrices
                    .GroupBy(p => p.event_zone_id)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // ⚡ Populate zones - gọi DateTime.UtcNow 1 lần
                var now = DateTime.UtcNow;

                foreach (var zone in zones)
                {
                    // Set current price theo thời gian hiện tại
                    if (priceLookup.TryGetValue(zone.event_zone_id, out var prices))
                    {
                        var currentPrice = prices.FirstOrDefault(p =>
                            (p.start_time <= now || p.start_time == null) &&
                            (p.end_time >= now || p.end_time == null));

                        if (currentPrice is not null)
                            zone.current_price = currentPrice.price;
                    }

                    // Set seats cho zone
                    if (zoneSectionLookup.TryGetValue(zone.event_zone_id, out var zoneSections))
                    {
                        var zoneSeats = new List<EventVenueSeatDto>();
                        foreach (var section in zoneSections)
                        {
                            if (seatLookup.TryGetValue(section.section_id, out var sectSeats))
                                zoneSeats.AddRange(sectSeats);
                        }

                        zone.seats = zoneSeats;
                    }
                }
            }

            result.zones = zones;
            // ✅ Cache toàn bộ result (static data)
            await cacheService.SetAsync(cacheKey, result, EventDetailCacheTtl, cancellationToken);

            // 🎯 STEP 5: Fetch seat status REAL-TIME (ngoài cache)
            await PopulateSeatStatusAsync(result, request.event_id, cancellationToken);

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<EventClientDetailDto?>().MessageSuccess(result, DomainMessageConstants.Event.GetDetailSuccess);
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<EventClientDetailDto?>().MessageError(e.Message);
        }
        finally
        {
        }
    }

    private async Task PopulateSeatStatusAsync(EventClientDetailDto result, long eventId, CancellationToken cancellationToken)
    {
        if (result?.zones == null || result.zones.Count == 0)
            return;

        try
        {
            // Collect toàn bộ seat IDs
            var allSeatIds = result.zones
                .SelectMany(z => z.seats ?? new List<EventVenueSeatDto>())
                .Select(s => s.seat_id)
                .Distinct()
                .ToArray();

            if (allSeatIds.Length == 0)
                return;

            // 🎯 Batch query seat status từ DB (REAL-TIME, không cache)
            var seatStatuses = (await unitOfWork.EventSeatInventory
                .GetBySeatIds<EventSeatInventoryDto>(new
                {
                    event_id = eventId,
                    seat_ids = allSeatIds
                }, cancellationToken)).ToList();

            var statusLookup = seatStatuses
                .GroupBy(s => s.seat_id)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault());

            // Populate status vào từng seat
            foreach (var zone in result.zones)
            {
                if (zone.seats?.Count > 0)
                {
                    foreach (var seat in zone.seats)
                    {
                        if (statusLookup.TryGetValue(seat.seat_id, out var status))
                        {
                            seat.event_seat_inventory_id = status?.event_seat_inventory_id;
                            seat.status = status?.seat_status ?? "available";
                            if (seat.status == "held" || seat.status == "locked")
                            {
                                seat.order_id = status?.current_hold_id;
                            }
                            else if (seat.status == "sold")
                            {
                                seat.order_id = status?.current_order_item_id;
                            }
                        }
                        else
                        {
                            // Default: available nếu không có record trong inventory
                            seat.status = "available";
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log nhưng không throw - seat status là optional
            // Client có thể handle nếu không có status
        }
    }

    private async Task RemoveEventDetailCacheAsync(long? eventId, CancellationToken cancellationToken)
    {
        if (eventId is not > 0)
            return;

        try
        {
            await cacheService.RemoveAsync(
                cacheKeyHelper.Build("ticketing:event:detail:v1", new EventGetByIdRequest { event_id = eventId.Value }),
                cancellationToken);
        }
        catch
        {
            // Cache invalidation fail should not break write flow.
        }
    }

    private async Task<string> GetEventClientCacheVersionAsync(CancellationToken cancellationToken)
    {
        try
        {
            var currentVersion = await cacheService.GetAsync<string>(EventClientCacheVersionKey, cancellationToken);
            if (!string.IsNullOrWhiteSpace(currentVersion))
                return currentVersion;

            var initialVersion = "v1";
            await cacheService.SetAsync(EventClientCacheVersionKey, initialVersion, EventClientCacheVersionTtl, cancellationToken);

            return initialVersion;
        }
        catch
        {
            return "v1";
        }
    }

    private async Task BumpEventClientCacheVersionAsync(CancellationToken cancellationToken)
    {
        try
        {
            var newVersion = Guid.NewGuid().ToString("N");
            await cacheService.SetAsync(EventClientCacheVersionKey, newVersion, EventClientCacheVersionTtl, cancellationToken);
        }
        catch
        {
            // Cache invalidation fail should not break write flow.
        }
    }
}