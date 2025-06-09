using HIV.DTOs;

namespace HIV.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<IEnumerable<MedicalRecordDto>> GetAllAsync();
        Task<MedicalRecordDto?> GetByIdAsync(int id);
        Task<MedicalRecordDto> CreateAsync(CreateMedicalRecordDto dto);
        Task<bool> UpdateAsync(int id, UpdateMedicalRecordDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
