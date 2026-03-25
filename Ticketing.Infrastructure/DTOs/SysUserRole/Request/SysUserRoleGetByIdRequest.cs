using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.SysUserRole.Request;

public class SysUserRoleGetByIdRequest
{
    [Required(ErrorMessage = "user_role_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "user_role_id phải lớn hơn 0")]
    public long user_role_id { get; set; }
}

