using HIV.DTOs;

namespace HIV.Interfaces
{
    public interface IDoctorInfoService
    {
        Task<IEnumerable<DoctorInfoDto>> GetAllAsync();
        Task<DoctorInfoDto?> GetByIdAsync(int doctorId);
        Task<DoctorInfoDto> CreateAsync(CreateDoctorInfoDto dto);
        Task<bool> UpdateAsync(int doctorId, UpdateDoctorInfoDto dto);
        Task<bool> DeleteAsync(int doctorId);
    }
}
