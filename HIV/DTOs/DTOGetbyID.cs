using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace HIV.DTOs
{
    public class DTOGetbyID : IValidatableObject
    {
        public int account_id { get; set; }

        public int user_id { get; set; }

        [Required]
        public string username { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

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

                if (!hasUpper || !hasLower || !hasDigit)
                    yield return new ValidationResult("Password must contain at least one uppercase letter, lowercase lettet, digit.", new[] { nameof(password_hash) });

            }
        }

        public DateTime created_at { get; set; }

        [Required]
        public string full_name { get; set; }

        [Required]
        [Phone]
        public string phone { get; set; }

        [Required]
        public string gender { get; set; }

        public DateOnly? birthdate { get; set; }
        public string role { get; set; }

        public string address { get; set; }

        public string status { get; set; } = "ACTIVE";

        public string? user_avatar { get; set; } = "patient.png";

    }
}
