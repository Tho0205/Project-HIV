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

        Task<IEnumerable<MedicalRecordDto>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<MedicalRecordDto>> GetByDoctorIdAsync(int doctorId);
        Task<bool> UpdateExamReference(int examId);
        Task<bool> UpdateCustomProtocolReference(int customProtocolId);
    }
}
