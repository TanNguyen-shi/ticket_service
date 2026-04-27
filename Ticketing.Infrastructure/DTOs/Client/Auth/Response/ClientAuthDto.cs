namespace Ticketing.Infrastructure.DTOs.Client.Auth.Response;

public class ClientAuthDto
{
    public string access_token { get; set; } = string.Empty;
    public string token_type { get; set; } = "Bearer";
    public int expires_in { get; set; }
    public ClientProfileDto customer { get; set; } = new();
}

public class ClientProfileDto
{
    public long customer_id { get; set; }
    public string username { get; set; } = string.Empty;
    public string? email { get; set; }
    public string? phone { get; set; }
    public string full_name { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
}
