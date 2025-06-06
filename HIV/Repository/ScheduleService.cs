
using HIV.DTOs;
using HIV.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoSWP391.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly AppDbContext _context;

        public ScheduleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ScheduleDto>> GetDoctorSchedulesAsync(int doctorId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Schedules
                .Include(s => s.Appointments)
                    .ThenInclude(a => a.Patient)
                .Where(s => s.DoctorId == doctorId);

            if (fromDate.HasValue)
                query = query.Where(s => s.ScheduledTime >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(s => s.ScheduledTime <= toDate.Value);

            var schedules = await query
                .OrderBy(s => s.ScheduledTime)
                .Select(s => new ScheduleDto
                {
                    ScheduleId = s.ScheduleId,
                    ScheduledTime = s.ScheduledTime,
                    Room = s.Room,
                    HasAppointment = s.Appointments.Any(),
                    PatientName = s.Appointments.FirstOrDefault() != null
                        ? s.Appointments.FirstOrDefault()!.Patient!.FullName
                        : null,
                    AppointmentNote = s.Appointments.FirstOrDefault() != null
                        ? s.Appointments.FirstOrDefault()!.Note
                        : null
                })
                .ToListAsync();

            return schedules;
        }

        public async Task<ScheduleDto?> GetScheduleByIdAsync(int scheduleId)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Appointments)
                    .ThenInclude(a => a.Patient)
                .Where(s => s.ScheduleId == scheduleId)
                .Select(s => new ScheduleDto
                {
                    ScheduleId = s.ScheduleId,
                    ScheduledTime = s.ScheduledTime,
                    Room = s.Room,
                    HasAppointment = s.Appointments.Any(),
                    PatientName = s.Appointments.FirstOrDefault() != null
                        ? s.Appointments.FirstOrDefault()!.Patient!.FullName
                        : null,
                    AppointmentNote = s.Appointments.FirstOrDefault() != null
                        ? s.Appointments.FirstOrDefault()!.Note
                        : null
                })
                .FirstOrDefaultAsync();

            return schedule;
        }

        public async Task<ScheduleDto> CreateScheduleAsync(CreateScheduleDto dto)
        {
            var schedule = new Schedule
            {
                DoctorId = dto.DoctorId,
                ScheduledTime = dto.ScheduledTime,
                Room = dto.Room
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            return await GetScheduleByIdAsync(schedule.ScheduleId)
                ?? throw new InvalidOperationException("Failed to retrieve created schedule");
        }

        public async Task<ScheduleDto?> UpdateScheduleAsync(int scheduleId, UpdateScheduleDto dto)
        {
            var schedule = await _context.Schedules.FindAsync(scheduleId);
            if (schedule == null)
                return null;

            schedule.ScheduledTime = dto.ScheduledTime ?? schedule.ScheduledTime;
            schedule.Room = dto.Room ?? schedule.Room;

            await _context.SaveChangesAsync();

            return await GetScheduleByIdAsync(scheduleId);
        }

        public async Task<bool> DeleteScheduleAsync(int scheduleId)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Appointments)
                .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);

            if (schedule == null)
                return false;

            // Không thể xóa schedule nếu đã có appointment
            if (schedule.Appointments.Any())
                return false;

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ScheduleDto>> GetAvailableSchedulesAsync(DateTime? date = null)
        {
            var query = _context.Schedules
                .Include(s => s.Appointments)
                .Include(s => s.Doctor)
                .Where(s => !s.Appointments.Any()); // Chỉ lấy schedule chưa có appointment

            if (date.HasValue)
            {
                var startDate = date.Value.Date;
                var endDate = startDate.AddDays(1);
                query = query.Where(s => s.ScheduledTime >= startDate && s.ScheduledTime < endDate);
            }

            var schedules = await query
                .OrderBy(s => s.ScheduledTime)
                .Select(s => new ScheduleDto
                {
                    ScheduleId = s.ScheduleId,
                    ScheduledTime = s.ScheduledTime,
                    Room = s.Room,
                    HasAppointment = false,
                    PatientName = null,
                    AppointmentNote = null
                })
                .ToListAsync();

            return schedules;
        }
    }
}