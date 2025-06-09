using HIV.Models;
using HIV.DTOs;

namespace HIV.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetUsersWithRoleAsync(string role);
        Task<User> GetUserByIdAsync(int userId);
        Task<List<UserWithAppointmentDto>> GetPatientAppointmentDetailsAsync();
    }
}