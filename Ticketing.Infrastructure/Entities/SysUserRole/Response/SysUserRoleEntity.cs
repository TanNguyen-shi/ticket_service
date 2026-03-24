namespace Ticketing.Infrastructure.Entities.SysUserRole.Response;

public class SysUserRoleEntity : BaseEntity
{
    public long user_role_id { get; set; }
    public long user_id { get; set; }
    public long role_id { get; set; }
    public DateTime? assigned_at { get; set; }
    public long? assigned_by { get; set; }
}

