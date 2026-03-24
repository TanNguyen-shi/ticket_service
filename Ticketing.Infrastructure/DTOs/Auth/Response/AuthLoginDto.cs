namespace Ticketing.Infrastructure.DTOs.Auth.Response;

public class AuthLoginDto
{
    public string access_token { get; set; } = string.Empty;
    public string token_type { get; set; } = "Bearer";
    public int expires_in { get; set; }
    public AuthUserProfileDto user { get; set; } = new();
}

public class AuthUserProfileDto
{
    public long user_id { get; set; }
    public string username { get; set; } = string.Empty;
    public string full_name { get; set; } = string.Empty;
    public string user_type { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
    public List<string> roles { get; set; } = [];
}