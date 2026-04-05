# SeatHold Module - Dependency Injection Setup

## Step 1: Add to InfrastructureConfigDI.cs

Add the following code to your `InfrastructureConfigDI.cs` file in the `ConfigureInfrastructureDI()` method:

```csharp
/// <summary>
/// Configure SeatHold UnitOfWork (Infrastructure Layer)
/// </summary>
services.AddScoped<ISeatHoldUnitOfWork>(provider =>
{
    var dapperContext = provider.GetRequiredService<DapperContext>();
    var contextAccessor = provider.GetRequiredService<DapperContextAccessor>();
    var dapperHelper = provider.GetRequiredService<IDapperProcedureHelper>();
    
    return new SeatHoldUnitOfWork(dapperContext, contextAccessor, dapperHelper);
});
```

## Step 2: Add Using Statements

Ensure the following using statements are present at the top of `InfrastructureConfigDI.cs`:

```csharp
using Ticketing.Infrastructure.Repositories.SeatHold;
using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Persistence.Helpers;
```

## Step 3: Usage Example in UseCase

Once registered, you can inject `ISeatHoldUnitOfWork` into your application services:

```csharp
public class HoldSeatUseCase
{
    private readonly ISeatHoldUnitOfWork _unitOfWork;
    
    public HoldSeatUseCase(ISeatHoldUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<string> ExecuteAsync(HoldSeatRequest request, CancellationToken cancellationToken)
    {
        // Open database connection
        await _unitOfWork.OpenAsync(cancellationToken);
        
        try
        {
            // Begin transaction
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            
            // Create seat hold
            var holdDto = new SeatHoldInsertDto
            {
                hold_code = GenerateHoldCode(),
                event_id = request.event_id,
                user_id = request.user_id,
                status = "active",
                hold_started_at = DateTime.UtcNow,
                hold_expires_at = DateTime.UtcNow.AddMinutes(10)
            };
            
            var holdId = await _unitOfWork.SeatHoldRepository.InsertAsync(holdDto, cancellationToken);
            
            // Create hold items for each seat
            foreach (var seatId in request.seat_ids)
            {
                var itemDto = new SeatHoldItemInsertDto
                {
                    hold_id = long.Parse(holdId),
                    event_seat_inventory_id = seatId,
                    seat_id = seatId,
                    zone_id = request.zone_id,
                    price_at_hold = request.price,
                    status = "active"
                };
                
                await _unitOfWork.SeatHoldItemRepository.InsertAsync(itemDto, cancellationToken);
            }
            
            // Commit transaction
            await _unitOfWork.CommitAsync(isCloseConnection: true, cancellationToken);
            
            return holdId;
        }
        catch (Exception ex)
        {
            // Rollback on error
            await _unitOfWork.RollbackAsync(isCloseConnection: true, cancellationToken);
            throw;
        }
    }
}
```

## Step 4: Verify DI Registration

Test that the DI is properly configured by running your application and checking that the dependencies resolve correctly.

## Complete Minimal Example

```csharp
public class ClientBookingController : ControllerBase
{
    private readonly ISeatHoldUnitOfWork _seatHoldUnitOfWork;
    
    public ClientBookingController(ISeatHoldUnitOfWork seatHoldUnitOfWork)
    {
        _seatHoldUnitOfWork = seatHoldUnitOfWork;
    }
    
    [HttpPost("api/client/booking/hold-seats")]
    public async Task<IActionResult> HoldSeats(
        [FromBody] HoldSeatsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _seatHoldUnitOfWork.OpenAsync(cancellationToken);
            await _seatHoldUnitOfWork.BeginTransactionAsync(cancellationToken);
            
            // Create hold
            var holdId = await _seatHoldUnitOfWork.SeatHoldRepository
                .InsertAsync(new SeatHoldInsertDto 
                { 
                    event_id = request.event_id,
                    user_id = request.user_id,
                    hold_code = $"HOLD{DateTime.UtcNow:yyyyMMddHHmmss}",
                    status = "active",
                    hold_started_at = DateTime.UtcNow,
                    hold_expires_at = DateTime.UtcNow.AddMinutes(10)
                }, 
                cancellationToken);
            
            await _seatHoldUnitOfWork.CommitAsync(cancellationToken: cancellationToken);
            
            return Ok(new { hold_id = holdId });
        }
        catch (Exception ex)
        {
            await _seatHoldUnitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return BadRequest(new { error = ex.Message });
        }
    }
}
```

## Troubleshooting

### Error: "Cannot resolve symbol 'ISeatHoldUnitOfWork'"
- Make sure the DI registration code is added to `InfrastructureConfigDI.cs`
- Verify `ConfigureServices()` calls `ConfigureinframstructureDI(services)`

### Error: "Database connection failed"
- Ensure PostgreSQL is running and accessible
- Check connection string in `appsettings.json`
- Verify stored procedures are created in the database

### Error: "Stored procedure not found"
- Run `database/functions/seat_hold_functions.sql` against your database
- Check database schema: `SELECT * FROM information_schema.routines WHERE routine_schema = 'ticketing'`

## Next Steps

1. ✅ Add DI registration to `InfrastructureConfigDI.cs`
2. ✅ Verify connection string in `appsettings.json`
3. ✅ Execute SQL functions: `seat_hold_functions.sql`
4. ⏳ Create Domain Services (optional)
5. ⏳ Create UseCases for booking workflow
6. ⏳ Create Controllers with API endpoints
7. ⏳ Create Integration Tests

## Related Files

- Entity: `Ticketing.Infrastructure/Entities/SeatHold/Response/SeatHoldEntity.cs`
- DTOs: `Ticketing.Infrastructure/DTOs/Client/SeatHold/**`
- Repositories: `Ticketing.Infrastructure/Repositories/SeatHold/**`
- SQL: `database/functions/seat_hold_functions.sql`
- Implementation Guide: `SEATHOLD_MODULE_IMPLEMENTATION.md`

