using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.SysRole.Request;

public class SysRoleDeleteRequest
{
    [Required(ErrorMessage = "role_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "role_id must be greater than 0")]
    public long role_id { get; set; }
}

