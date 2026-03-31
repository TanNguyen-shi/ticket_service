using System.Text.Json.Serialization;

namespace Ticketing.Infrastructure.DTOs.SysUser.Response;

public class SysUserDto : AuditTable
{
    public long user_id { get; set; }
    public string username { get; set; } = string.Empty;
    public string? email { get; set; }
    public string? phone { get; set; }
    [JsonIgnore] public string password_hash { get; set; } = string.Empty;
    public string full_name { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
}