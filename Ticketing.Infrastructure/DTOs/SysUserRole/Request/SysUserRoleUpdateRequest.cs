using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.SysUserRole.Request;

public class SysUserRoleUpdateRequest
{
    [Required(ErrorMessage = "user_role_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "user_role_id must be greater than 0")]
    public long user_role_id { get; set; }

    [Required(ErrorMessage = "user_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "user_id must be greater than 0")]
    public long user_id { get; set; }

    [Required(ErrorMessage = "role_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "role_id must be greater than 0")]
    public long role_id { get; set; }

    public DateTime? assigned_at { get; set; }
}

