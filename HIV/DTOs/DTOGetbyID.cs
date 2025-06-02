using System.ComponentModel.DataAnnotations;

namespace HIV.DTOs
{
    public class DTOGetbyID
    {
        public int account_id { get; set; }

        public int user_id { get; set; }

        [Required]
        public string username { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        public string password_hash { get; set; }

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
    }
}
