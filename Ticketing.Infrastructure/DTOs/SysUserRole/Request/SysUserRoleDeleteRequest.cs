using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.SysUserRole.Request;

public class SysUserRoleDeleteRequest
{
    [Required(ErrorMessage = "user_role_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "user_role_id must be greater than 0")]
    public long user_role_id { get; set; }
}

