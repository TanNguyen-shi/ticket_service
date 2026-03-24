namespace Ticketing.Infrastructure.Entities.SysUser.Response;

public class SysUserEntity : BaseEntity
{
    public long user_id { get; set; }
    public string username { get; set; } = string.Empty;
    public string? email { get; set; }
    public string? phone { get; set; }
    public string password_hash { get; set; } = string.Empty;
    public string full_name { get; set; } = string.Empty;
    public string user_type { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
    public DateTime? last_login_at { get; set; }
}

