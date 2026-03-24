using Microsoft.AspNetCore.Mvc.ModelBinding;
using Ticketing.Infrastructure.DTOs;

namespace Ticketing.Infrastructure.Extensions;

public static class ValidationResponseExtensions
{
    public static List<BaseError> ToErrors(ModelStateDictionary modelState)
    {
        return modelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors.Select(e => new BaseError(
                x.Key,
                string.IsNullOrWhiteSpace(e.ErrorMessage) ? "invalid value" : e.ErrorMessage
            )))
            .ToList();
    }

    public static ResponseMessage<object> ToResponse(
        ModelStateDictionary modelState,
        string message = "invalid input")
    {
        return ResponseMessage<object>.Error(message, ToErrors(modelState));
    }
}