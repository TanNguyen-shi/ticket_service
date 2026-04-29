using Ticketing.Application.UseCases.Client.Booking.Interfaces;

namespace TicketingSystem.BackgroundServices;

/// <summary>
/// Background service chạy mỗi 60 giây để tự động nhả ghế của các phiên giữ chỗ đã hết hạn 10 phút.
///
/// Lý do dùng IServiceScopeFactory thay vì inject IBookingUseCases trực tiếp:
/// BackgroundService là singleton, trong khi IBookingUseCases là scoped.
/// Mỗi lần chạy cần tạo một scope riêng để đảm bảo UnitOfWork và connection được quản lý đúng.
/// </summary>
public class SeatHoldExpiryBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<SeatHoldExpiryBackgroundService> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(60);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("SeatHoldExpiryBackgroundService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(Interval, stoppingToken);

            if (stoppingToken.IsCancellationRequested) break;

            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var bookingUseCases = scope.ServiceProvider.GetRequiredService<IBookingUseCases>();
                await bookingUseCases.ReleaseExpiredHoldsAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi tự động nhả ghế hết hạn");
                // Không dừng service — tiếp tục ở lần chạy tiếp theo
            }
        }

        logger.LogInformation("SeatHoldExpiryBackgroundService stopped");
    }
}
