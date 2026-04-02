using Ticketing.Application.Model.DTOs;
using Ticketing.Domain.Constants;
using Ticketing.Domain.Domain.Event.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Client.Event.Request;
using Ticketing.Infrastructure.DTOs.Client.Event.Response;
using Ticketing.Infrastructure.DTOs.Event.Request;
using Ticketing.Infrastructure.DTOs.Event.Response;
using Ticketing.Infrastructure.Entities;
using Ticketing.Infrastructure.Entities.Event.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Helpers.Interfaces;
using Ticketing.Infrastructure.Repositories.Event;

namespace Ticketing.Domain.Domain.Event;

public class EventDomainService(
    IEventUnitOfWork unitOfWork,
    ICacheService cacheService,
    ICacheKeyHelper cacheKeyHelper)
    : BaseService<IEventRepository, EventEntity>(unitOfWork.Event, TicketingTypeEnum.Event), IEventDomainService
{
    private static readonly TimeSpan EventDetailCacheTtl = TimeSpan.FromMinutes(5);
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
        try
        {
            var cacheKey = cacheKeyHelper.Build("ticketing:event:detail:v1", request);
            var cachedResult = await cacheService.GetAsync<EventClientDetailDto>(cacheKey, cancellationToken);
            if (cachedResult is not null)
            {
                return new ResponseMessage<EventClientDetailDto?>().MessageSuccess(cachedResult, DomainMessageConstants.Event.GetDetailSuccess);
            }

            var result = await _repository.GetAsync<EventClientDetailDto>(new
            {
                event_id = request.event_id
            }, cancellationToken);

            if (result is null)
                return new ResponseMessage<EventClientDetailDto?>().MessageError(DomainMessageConstants.Event.NotFound);

            //Lấy thông tin zones
            var zones = (await unitOfWork.EventZone.GetByEventId<EventZoneDto>(new
            {
                event_id = request.event_id
            }, cancellationToken)).ToList();

            foreach (var zone in zones)
            {
                zone.prices = (await unitOfWork.EventZonePrice.GetByZoneId<EventZonePriceDto>(new
                {
                    event_zone_id = zone.event_zone_id
                }, cancellationToken)).ToList();
            }

            result.zones = zones;

            await cacheService.SetAsync(cacheKey, result, EventDetailCacheTtl, cancellationToken);

            return new ResponseMessage<EventClientDetailDto?>().MessageSuccess(result, DomainMessageConstants.Event.GetDetailSuccess);
        }
        catch (Exception e)
        {
            return new ResponseMessage<EventClientDetailDto?>().MessageError(e.Message);
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