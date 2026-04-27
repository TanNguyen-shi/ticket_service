using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.Client.Auth.Request;

public class ClientRegisterRequest : IValidatableObject
{
    [Required(ErrorMessage = "username là bắt buộc")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "username phải có từ 3 đến 50 ký tự")]
    public string username { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "email phải có tối đa 100 ký tự")]
    [EmailAddress(ErrorMessage = "email không đúng định dạng")]
    public string? email { get; set; }

    [StringLength(20, ErrorMessage = "phone phải có tối đa 20 ký tự")]
    public string? phone { get; set; }

    [Required(ErrorMessage = "password là bắt buộc")]
    [StringLength(200, MinimumLength = 6, ErrorMessage = "password phải có từ 6 đến 200 ký tự")]
    public string password { get; set; } = string.Empty;

    [Required(ErrorMessage = "full_name là bắt buộc")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "full_name phải có từ 2 đến 100 ký tự")]
    public string full_name { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            yield return new ValidationResult("username không được để trống", new[] { nameof(username) });
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            yield return new ValidationResult("password không được để trống", new[] { nameof(password) });
        }

        if (string.IsNullOrWhiteSpace(full_name))
        {
            yield return new ValidationResult("full_name không được để trống", new[] { nameof(full_name) });
        }
    }
}
