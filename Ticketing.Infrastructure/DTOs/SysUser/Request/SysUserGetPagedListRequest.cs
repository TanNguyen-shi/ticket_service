namespace Ticketing.Infrastructure.DTOs.SysUser.Request;

public class SysUserGetPagedListRequest : BaseFilterPaging
{
    public string? status { get; set; } = string.Empty;
}

