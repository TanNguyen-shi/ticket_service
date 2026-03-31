using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.SysUser.Request;

public class SysUserUpdateRequest
{
    [Required(ErrorMessage = "user_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "user_id phải lớn hơn 0")]
    public long user_id { get; set; }

    [EmailAddress(ErrorMessage = "email không hợp lệ")]
    [StringLength(255, ErrorMessage = "email phải có tối đa 255 ký tự")]
    public string? email { get; set; }

    [StringLength(20, ErrorMessage = "phone phải có tối đa 20 ký tự")]
    public string? phone { get; set; }

    [Required(ErrorMessage = "full_name là bắt buộc")]
    [StringLength(255, ErrorMessage = "full_name phải có tối đa 255 ký tự")]
    public string full_name { get; set; } = string.Empty;

    [Required(ErrorMessage = "status là bắt buộc")]
    [StringLength(30, ErrorMessage = "status phải có tối đa 30 ký tự")]
    public string status { get; set; } = string.Empty;
}

