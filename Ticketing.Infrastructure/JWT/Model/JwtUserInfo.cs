namespace Ticketing.Infrastructure.JWT.Model;

public class JwtUserInfo
{
    public long user_id { get; set; }
    public string username { get; set; } = string.Empty;
    public string full_name { get; set; } = string.Empty;
    public string user_type { get; set; } = string.Empty;
    public List<string> roles { get; set; } = [];
}