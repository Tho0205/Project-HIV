using System.ComponentModel.DataAnnotations;

namespace HIV.DTOs
{
    public class DTOGetPatient
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }
        public DateTime created_at { get; set; }

        [Required]
        public string full_name { get; set; }

        [Required]
        [Phone]
        public string phone { get; set; }

        [Required]
        public string gender { get; set; }

        public DateOnly? birthdate { get; set; }

        public string address { get; set; }
        public string? UserAvatar { get; set; }

        public string status { get; set; }

    }
}
