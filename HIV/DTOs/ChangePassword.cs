using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace HIV.DTOs
{
    public class ChangePassword : IValidatableObject
    {
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Password must be between 4 and 50 characters.")]
        public string password_hash { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(password_hash))
            {
                // Example: Require at least one uppercase, one lowercase, one digit, and one special character
                var hasUpper = new Regex(@"[A-Z]+").IsMatch(password_hash);
                var hasLower = new Regex(@"[a-z]+").IsMatch(password_hash);
                var hasDigit = new Regex(@"\d+").IsMatch(password_hash);

                if (!hasUpper)
                    yield return new ValidationResult("Password must contain at least one uppercase letter.", new[] { nameof(password_hash) });
                if (!hasLower)
                    yield return new ValidationResult("Password must contain at least one lowercase letter.", new[] { nameof(password_hash) });
                if (!hasDigit)
                    yield return new ValidationResult("Password must contain at least one digit.", new[] { nameof(password_hash) });
            }
        }
    }

}
