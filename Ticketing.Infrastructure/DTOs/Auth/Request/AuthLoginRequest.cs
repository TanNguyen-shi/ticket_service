using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.Auth.Request;

public class AuthLoginRequest : IValidatableObject
{
    [Required(ErrorMessage = "username is required")]
    [StringLength(50, ErrorMessage = "username must be less than or equal to 50 characters")]
    public string username { get; set; } = string.Empty;

    [Required(ErrorMessage = "password is required")]
    [StringLength(200, ErrorMessage = "password must be less than or equal to 200 characters")]
    public string password { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            yield return new ValidationResult("username must not be empty", new[] { nameof(username) });
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            yield return new ValidationResult("password must not be empty", new[] { nameof(password) });
        }
    }
}