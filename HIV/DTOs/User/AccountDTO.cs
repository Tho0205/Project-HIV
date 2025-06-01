using HIV.Models;

namespace HIV.DTOs
{
    public class AccountDTO
    {
        public int AccountId { get; set; }

        public string Username { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string? Email { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual User? UserTable { get; set; }
    }
}
