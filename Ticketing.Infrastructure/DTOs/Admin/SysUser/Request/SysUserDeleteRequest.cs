using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.SysUser.Request;

public class SysUserDeleteRequest
{
    [Required(ErrorMessage = "user_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "user_id phải lớn hơn 0")]
    public long user_id { get; set; }
}

