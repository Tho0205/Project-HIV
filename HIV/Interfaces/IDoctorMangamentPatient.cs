using HIV.DTOs.DoctorPatient;

namespace HIV.Interfaces
{
    public interface IDoctorMangamentPatient
    {
        Task<DoctorPatientsResponseDto> GetDoctorPatientsAsync(
            int doctorId,
            string sortBy = "full_name",
            string order = "asc",
            int page = 1,
            int pageSize = 8);

        Task<bool> UpdatePatientInfoAsync(int accountId, DoctorPatientUpdateDto dto);

        Task<DoctorPatientStatsDto> GetDoctorPatientStatsAsync(int doctorId);

        Task<PatientHistoryDto> GetPatientHistoryAsync(int patientId, int doctorId);

        Task<bool> CanDoctorAccessPatientAsync(int doctorId, int patientId);

        Task<DoctorPatientListDto?> GetPatientDetailAsync(int patientId, int doctorId);
    }
}