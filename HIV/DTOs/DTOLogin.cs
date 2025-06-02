using System.ComponentModel.DataAnnotations;

namespace HIV.DTOs
{
    public class DTOLogin
    {
        [Required]
        public string identifier { get; set; } // username hoặc email

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string password_hash { get; set; }
    }
}
