using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.Auth.Request;

public class AuthLoginRequest : IValidatableObject
{
    [Required(ErrorMessage = "username là bắt buộc")]
    [StringLength(50, ErrorMessage = "username phải có tối đa 50 ký tự")]
    public string username { get; set; } = string.Empty;

    [Required(ErrorMessage = "password là bắt buộc")]
    [StringLength(200, ErrorMessage = "password phải có tối đa 200 ký tự")]
    public string password { get; set; } = string.Empty;

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
    }
}