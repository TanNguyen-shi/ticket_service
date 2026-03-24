namespace Ticketing.Infrastructure.Entities.SysRole.Response;

public class SysRoleEntity : BaseEntity
{
    public long role_id { get; set; }
    public string role_code { get; set; } = string.Empty;
    public string role_name { get; set; } = string.Empty;
    public string? description { get; set; }
    public string status { get; set; } = string.Empty;
}

