using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.SysRole.Request;

public class SysRoleCreateRequest
{
    [Required(ErrorMessage = "role_code là bắt buộc")]
    [StringLength(50, ErrorMessage = "role_code phải có tối đa 50 ký tự")]
    public string role_code { get; set; } = string.Empty;

    [Required(ErrorMessage = "role_name là bắt buộc")]
    [StringLength(255, ErrorMessage = "role_name phải có tối đa 255 ký tự")]
    public string role_name { get; set; } = string.Empty;

    public string? description { get; set; }

    [Required(ErrorMessage = "status là bắt buộc")]
    [StringLength(30, ErrorMessage = "status phải có tối đa 30 ký tự")]
    public string status { get; set; } = string.Empty;
}

