namespace Ticketing.Infrastructure.DTOs.SysRole.Response;

public class SysRoleDto : AuditTable
{
    public long role_id { get; set; }
    public string role_code { get; set; } = string.Empty;
    public string role_name { get; set; } = string.Empty;
    public string? description { get; set; }
    public string status { get; set; } = string.Empty;
    public string? created_by_name { get; set; }
    public string? updated_by_name { get; set; }
}

