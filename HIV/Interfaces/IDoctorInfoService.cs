using HIV.DTOs;

namespace HIV.Interfaces
{
    public interface IDoctorInfoService
    {
        Task<IEnumerable<DoctorInfoDto>> GetAllAsync();
        Task<DoctorInfoDto?> GetByIdAsync(int doctorId);
        Task<bool> UpdateAsync(int doctorId, UpdateDoctorInfoDto dto);
        Task<bool> DeleteAsync(int doctorId);
        Task<int> SyncDoctorUsersAsync(); // NEW: Method to sync users with Doctor role
    }
}