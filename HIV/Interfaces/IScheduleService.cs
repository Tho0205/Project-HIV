
using HIV.DTOs;

namespace DemoSWP391.Services
{
    public interface IScheduleService
    {
        Task<List<ScheduleDto>> GetDoctorSchedulesAsync(int doctorId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<ScheduleDto?> GetScheduleByIdAsync(int scheduleId);
        Task<ScheduleDto> CreateScheduleAsync(CreateScheduleDto dto);
        Task<ScheduleDto?> UpdateScheduleAsync(int scheduleId, UpdateScheduleDto dto);
        Task<bool> DeleteScheduleAsync(int scheduleId);
        Task<List<ScheduleDto>> GetAvailableSchedulesAsync(DateTime? date = null);

        Task<List<ScheduleDto>> GetActiveSchedulesAsync(DateTime? fromDate = null, DateTime? toDate = null);

    }
}