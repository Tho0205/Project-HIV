using HIV.Models;

namespace HIV.Interfaces
{
    public interface IAdminManagementAccount
    {
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task<Account> GetAccountByIdAsync(int accountId);
        Task<Account> GetAccountInfoAsync(int accountId);
        Task<Account> CreateAccountAsync(Account account);
        Task<Account> UpdateAccountAsync(Account account);
        Task<bool> DeleteAccountAsync(int accountId);
        Task<bool> UpdateAccountStatusAsync(int accountId, string status);
        Task<IEnumerable<Account>> GetAccountsByStatusAsync(string status);
        Task<Account> GetAccountByUsernameAsync(string username);
        Task<bool> AccountExistsAsync(string username, string email);
    }
}