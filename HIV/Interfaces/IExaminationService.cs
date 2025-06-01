
using HIV.DTOs;

namespace DemoSWP391.Services
{
    public interface IExaminationService
    {
        Task<List<ExaminationResultDto>> GetPatientExaminationResultsAsync(int patientId);
        Task<ExaminationResultDto?> GetExaminationByIdAsync(int examId);
        Task<ExaminationResultDto> CreateExaminationAsync(CreateExaminationDto dto);
        Task<ExaminationResultDto?> UpdateExaminationAsync(int examId, UpdateExaminationDto dto);
        Task<bool> DeleteExaminationAsync(int examId);
    }
}