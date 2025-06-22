using HIV.Interfaces;
using HIV.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DemoSWP391.Services
{
    public class AdminAccountService : IAdminManagementAccount
    {
        private readonly AppDbContext _context;

        public AdminAccountService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await _context.Accounts
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Account> GetAccountByIdAsync(int accountId)
        {
            return await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountId == accountId);
        }

        public async Task<Account> GetAccountInfoAsync(int accountId)
        {
            return await _context.Accounts
                .Include(a => a.User)
                    .ThenInclude(u => u.DoctorInfo)
                .FirstOrDefaultAsync(a => a.AccountId == accountId);
        }

        public async Task<Account> CreateAccountAsync(Account account)
        {
            // Hash password trước khi lưu
            account.PasswordHash = HashPassword(account.PasswordHash);
            account.CreatedAt = DateTime.UtcNow;

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            // Tạo User mới với role từ account.User
            var user = new User
            {
                AccountId = account.AccountId,
                Role = account.User?.Role ?? "Patient", // Mặc định là Patient nếu không có role
                Status = "ACTIVE",
                FullName = "", // Có thể thêm field này vào form nếu cần
                Phone = "",
                Address = ""
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Load lại account với User mới tạo
            return await GetAccountByIdAsync(account.AccountId);
        }

        public async Task<Account> UpdateAccountAsync(Account account)
        {
            var existingAccount = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountId == account.AccountId);

            if (existingAccount == null)
                return null;

            existingAccount.Username = account.Username;
            existingAccount.Email = account.Email;
            existingAccount.Status = account.Status;

            // Cập nhật password nếu có
            if (!string.IsNullOrEmpty(account.PasswordHash))
            {
                existingAccount.PasswordHash = HashPassword(account.PasswordHash);
            }

            // Cập nhật role
            if (existingAccount.User != null && !string.IsNullOrEmpty(account.User?.Role))
            {
                existingAccount.User.Role = account.User.Role;
            }

            await _context.SaveChangesAsync();
            return existingAccount;
        }

        public async Task<bool> DeleteAccountAsync(int accountId)
        {
            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountId == accountId);

            if (account == null)
                return false;

            // Xóa User trước (nếu có) do foreign key constraint
            if (account.User != null)
            {
                // Xóa các related entities của User trước
                var user = await _context.Users
                    .Include(u => u.Blogs)
                    .Include(u => u.Comments)
                    .Include(u => u.Schedules)
                    .Include(u => u.AppointmentsAsPatient)
                    .Include(u => u.AppointmentsAsDoctor)
                    .FirstOrDefaultAsync(u => u.UserId == account.User.UserId);

                if (user != null)
                {
                    // Xóa blogs
                    _context.Blogs.RemoveRange(user.Blogs);
                    // Xóa comments
                    _context.Comments.RemoveRange(user.Comments);
                    // Xóa schedules
                    _context.Schedules.RemoveRange(user.Schedules);

                    _context.Users.Remove(user);
                }
            }

            // Sau đó xóa Account
            _context.Accounts.Remove(account);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAccountStatusAsync(int accountId, string status)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
                return false;

            account.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Account>> GetAccountsByStatusAsync(string status)
        {
            return await _context.Accounts
                .Include(a => a.User)
                .Where(a => a.Status == status)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Account> GetAccountByUsernameAsync(string username)
        {
            return await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Username == username);
        }

        public async Task<bool> AccountExistsAsync(string username, string email)
        {
            return await _context.Accounts
                .AnyAsync(a => a.Username == username || (!string.IsNullOrEmpty(email) && a.Email == email));
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}