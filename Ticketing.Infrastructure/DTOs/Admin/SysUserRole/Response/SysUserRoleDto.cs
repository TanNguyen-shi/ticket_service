namespace Ticketing.Infrastructure.DTOs.SysUserRole.Response;

public class SysUserRoleDto
{
    public long user_role_id { get; set; }
    public long user_id { get; set; }
    public string? username { get; set; }
    public string? user_full_name { get; set; }
    public long role_id { get; set; }
    public string? role_code { get; set; }
    public string? role_name { get; set; }
    public DateTime? assigned_at { get; set; }
    public long? assigned_by { get; set; }
    public string? assigned_by_name { get; set; }
}

