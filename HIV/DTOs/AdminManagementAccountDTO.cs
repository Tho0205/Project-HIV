namespace HIV.DTOs
{
    public class AccountDto
    {
        public int AccountId { get; set; }
        public string Username { get; set; } = null!;
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "ACTIVE";
        public UserDto? User { get; set; }
    }

    public class AccountInfoDto
    {
        public int AccountId { get; set; }
        public string Username { get; set; } = null!;
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "ACTIVE";
        public UserInfoDto? User { get; set; }
    }

    public class CreateAccountDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Email { get; set; }
        public string Status { get; set; } = "ACTIVE";
        public string? Role { get; set; }
    }

    public class UpdateAccountDto
    {
        public int AccountId { get; set; }
        public string Username { get; set; } = null!;
        public string? Email { get; set; }
        public string Status { get; set; } = "ACTIVE";
        public string? Password { get; set; }
        public string? Role { get; set; }
    }

    public class UpdateAccountStatusDto
    {
        public int AccountId { get; set; }
        public string Status { get; set; } = null!;
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string Status { get; set; }
    }

    public class UserInfoDto
    {
        public int UserId { get; set; }
        public int AccountId { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? UserAvatar { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public DateOnly? Birthdate { get; set; }
        public string? Role { get; set; }
        public string Status { get; set; }
    }
}