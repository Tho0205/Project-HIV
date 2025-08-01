using HIV.DTOs;

namespace HIV.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<IEnumerable<MedicalRecordDto>> GetAllAsync();
        Task<MedicalRecordDto?> GetByIdAsync(int id);
        Task<MedicalRecordDto> CreateAsync(CreateMedicalRecordDto dto);
        Task<IEnumerable<MedicalRecordDto>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<MedicalRecordDto>> GetByPatientIdAsync(int patientId);
        Task<bool> UpdateAsync(int id, UpdateMedicalRecordDto dto);
        Task<bool> DeleteAsync(int id);

        // Phương thức mới để lấy thông tin chi tiết bao gồm Examination và ARV Protocol
        Task<MedicalRecordDetailDto?> GetDetailByIdAsync(int id);

        Task<MedicalRecordDto> CreateByAppointmentIdAsync(CreateMedicalRecordByAppointmentDto dto);
    }
}