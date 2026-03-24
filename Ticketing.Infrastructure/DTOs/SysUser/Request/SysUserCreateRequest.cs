using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.SysUser.Request;

public class SysUserCreateRequest
{
    [Required(ErrorMessage = "username is required")]
    [StringLength(50, ErrorMessage = "username must be less than or equal to 50 characters")]
    public string username { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "email is invalid")]
    [StringLength(255, ErrorMessage = "email must be less than or equal to 255 characters")]
    public string? email { get; set; }

    [StringLength(20, ErrorMessage = "phone must be less than or equal to 20 characters")]
    public string? phone { get; set; }

    [Required(ErrorMessage = "password_hash is required")]
    public string password_hash { get; set; } = string.Empty;

    [Required(ErrorMessage = "full_name is required")]
    [StringLength(255, ErrorMessage = "full_name must be less than or equal to 255 characters")]
    public string full_name { get; set; } = string.Empty;

    [Required(ErrorMessage = "user_type is required")]
    [StringLength(30, ErrorMessage = "user_type must be less than or equal to 30 characters")]
    public string user_type { get; set; } = string.Empty;

    [Required(ErrorMessage = "status is required")]
    [StringLength(30, ErrorMessage = "status must be less than or equal to 30 characters")]
    public string status { get; set; } = string.Empty;

    public DateTime? last_login_at { get; set; }
}

