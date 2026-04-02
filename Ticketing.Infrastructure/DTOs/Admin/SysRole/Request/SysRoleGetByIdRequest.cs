using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.SysRole.Request;

public class SysRoleGetByIdRequest
{
    [Required(ErrorMessage = "role_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "role_id phải lớn hơn 0")]
    public long role_id { get; set; }
}

