using System.Net;
using System.Text.Json;
using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;

namespace TicketingSystem.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred.");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = ResponseMessage<string>.Error("Internal Server Error", ex.Message);

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}