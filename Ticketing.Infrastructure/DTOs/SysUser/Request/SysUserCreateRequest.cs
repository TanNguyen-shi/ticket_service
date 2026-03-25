using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.SysUser.Request;

public class SysUserCreateRequest
{
    [Required(ErrorMessage = "username là bắt buộc")]
    [StringLength(50, ErrorMessage = "username phải có tối đa 50 ký tự")]
    public string username { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "email không hợp lệ")]
    [StringLength(255, ErrorMessage = "email phải có tối đa 255 ký tự")]
    public string? email { get; set; }

    [StringLength(20, ErrorMessage = "phone phải có tối đa 20 ký tự")]
    public string? phone { get; set; }

    [Required(ErrorMessage = "password_hash là bắt buộc")]
    public string password_hash { get; set; } = string.Empty;

    [Required(ErrorMessage = "full_name là bắt buộc")]
    [StringLength(255, ErrorMessage = "full_name phải có tối đa 255 ký tự")]
    public string full_name { get; set; } = string.Empty;

    [Required(ErrorMessage = "user_type là bắt buộc")]
    [StringLength(30, ErrorMessage = "user_type phải có tối đa 30 ký tự")]
    public string user_type { get; set; } = string.Empty;

    [Required(ErrorMessage = "status là bắt buộc")]
    [StringLength(30, ErrorMessage = "status phải có tối đa 30 ký tự")]
    public string status { get; set; } = string.Empty;

    public DateTime? last_login_at { get; set; }
}

