namespace Ticketing.Infrastructure.DTOs.SysRole.Request;

public class SysRoleGetPagedListRequest : BaseFilterPaging
{
    public string? status { get; set; } = string.Empty;
}

