namespace Ticketing.Infrastructure.DTOs.SysUserRole.Request;

public class SysUserRoleGetPagedListRequest : BaseFilterPaging
{
    public long user_id { get; set; } = -1;
    public long role_id { get; set; } = -1;
}

