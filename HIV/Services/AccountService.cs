using HIV.Interfaces;
using HIV.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HIV.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;

        public AccountService(AppDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<string> LoginWithGoogleAsync(ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null)
            {
                throw new ArgumentNullException(nameof(claimsPrincipal));
            }

            // những dữ liệu đc lấy từ google thông qua ClaimTypes
            var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            var name = claimsPrincipal.FindFirstValue(ClaimTypes.Name);
            var googleId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var avatar = claimsPrincipal.FindFirstValue("urn:google:picture");


            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Không Tìm Thấy Tài Khoản Google Trong claims");
            }

            // coi thử nó có tồn tại hay ko 
            var accountgg = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Email == email);

            Account account;
            User user;

            if (accountgg == null)
            {
                // tạo ra account trước để lấy Account_id
                account = new Account
                {
                    Username = email,
                    Email = email,
                    PasswordHash = "", 
                    CreatedAt = DateTime.UtcNow,
                    Status = "ACTIVE"
                };

                _context.Accounts.Add(account);
                await _context.SaveChangesAsync(); // tạo nó dưới database

                //sau khi account đc tạo thành công thì mới tạo tiếp user 
                user = new User
                {
                    AccountId = account.AccountId,
                    FullName = name ?? email,
                    Role = "Patient", 
                    Status = "ACTIVE",
                    UserAvatar = avatar ?? "patient.png"
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                account.User = user;
            }
            else
            {
                account = accountgg;
                user = accountgg.User;

                if (user == null)
                {
                    // Create user if account exists but user doesn't
                    user = new User
                    {
                        AccountId = account.AccountId,
                        FullName = name ?? email,
                        Role = "Patient",
                        Status = "ACTIVE",
                        UserAvatar = avatar ?? "patient.png"
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    account.User = user;
                }
            }

            // tạo ra token từ những thông tin trên
            var token = _jwtService.GenerateToken(
                user.FullName ?? email,
                user.Role ?? "Patient",
                user.UserId,
                account.AccountId,
                user.UserAvatar ?? "patient.png"
            );

            return token;
        }
    }
}