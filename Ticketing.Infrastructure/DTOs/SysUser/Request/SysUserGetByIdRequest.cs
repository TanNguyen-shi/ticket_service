using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.SysUser.Request;

public class SysUserGetByIdRequest
{
    [Required(ErrorMessage = "user_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "user_id must be greater than 0")]
    public long user_id { get; set; }
}

