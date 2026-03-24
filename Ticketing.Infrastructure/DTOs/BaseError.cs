namespace Ticketing.Infrastructure.DTOs;

public class BaseError
{
    public BaseError()
    {
    }

    public BaseError(string property_message, string error_message)
    {
        this.property_message = property_message;
        this.error_message = error_message;
    }

    public string property_message { get; set; } = string.Empty;
    public string error_message { get; set; } = string.Empty;
}