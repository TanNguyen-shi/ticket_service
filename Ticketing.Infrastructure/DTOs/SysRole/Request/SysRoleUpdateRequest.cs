using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.SysRole.Request;

public class SysRoleUpdateRequest
{
    [Required(ErrorMessage = "role_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "role_id must be greater than 0")]
    public long role_id { get; set; }

    [Required(ErrorMessage = "role_code is required")]
    [StringLength(50, ErrorMessage = "role_code must be less than or equal to 50 characters")]
    public string role_code { get; set; } = string.Empty;

    [Required(ErrorMessage = "role_name is required")]
    [StringLength(255, ErrorMessage = "role_name must be less than or equal to 255 characters")]
    public string role_name { get; set; } = string.Empty;

    public string? description { get; set; }

    [Required(ErrorMessage = "status is required")]
    [StringLength(30, ErrorMessage = "status must be less than or equal to 30 characters")]
    public string status { get; set; } = string.Empty;
}

