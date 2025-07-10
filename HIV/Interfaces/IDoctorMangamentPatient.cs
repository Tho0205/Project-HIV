using HIV.DTOs.DoctorPatient;

namespace HIV.Interfaces
{
    public interface IDoctorMangamentPatient
    {
        //Task<DoctorPatientsResponseDto> GetDoctorPatientsAsync(
        //    int doctorId,
        //    string sortBy = "full_name",
        //    string order = "asc",
        //    int page = 1,
        //    int pageSize = 8);

        Task<DoctorPatientsResponseDto> GetDoctorPatientsAsync(
            int doctorId,
            DateTime? scheduleDate = null,
            bool hasScheduleOnly = false,
            string sortBy = "full_name",
            string order = "asc",
            int page = 1,
            int pageSize = 8);

        Task<DoctorPatientsResponseDto> GetAllPatientsAsync(
            string? searchTerm = null,
            string sortBy = "full_name",
            string order = "asc",
            int page = 1,
            int pageSize = 10);

        Task<DoctorPatientStatsDto> GetDoctorPatientStatsAsync(int doctorId);

        Task<PatientHistoryDto> GetPatientHistoryAsync(int patientId);

        Task<bool> CanDoctorAccessPatientAsync(int doctorId, int patientId);

        Task<DoctorPatientListDto?> GetPatientDetailAsync(int patientId, int doctorId);

    }
}