using HIV.DTOs;

namespace HIV.Interfaces
{
    public interface IHIVExaminationService
    {
        Task<List<PatientListDTO>> GetPatientsAsync(int page, int pageSize);
        Task<List<ExaminationDTO>> GetPatientExaminationsAsync(int patientId);
        Task<ExaminationDTO> SaveExaminationAsync(ExaminationFormDTO dto);
        Task<bool> DeleteExaminationAsync(int examId);
        Task<List<object>> GetDoctorsAsync();
        Task<bool> UpdateExamAsync(int examId, HIVExaminationDto dto);
    }
}