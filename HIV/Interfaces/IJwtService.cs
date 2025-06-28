using System.Security.Claims;

namespace HIV.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(string fullName, string role, int userId, int accountId, string user_avatar);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
