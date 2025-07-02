using Microsoft.AspNetCore.Identity.Data;
using System.Security.Claims;

namespace HIV.Interfaces
{
    public interface IAccountService
    {
        Task<string> LoginWithGoogleAsync(ClaimsPrincipal claimsPrincipal);
    }
}
