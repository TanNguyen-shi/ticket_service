namespace Ticketing.Infrastructure.DTOs.SysUser.Request;

public class SysUserGetPagedListRequest : BaseFilterPaging
{
    public string? user_type { get; set; } = string.Empty;
    public string? status { get; set; } = string.Empty;
}

