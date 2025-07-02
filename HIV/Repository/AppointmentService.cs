using HIV.DTOs;
using HIV.DTOs.DTOAppointment;
using HIV.DTOs.DTOSchedule;
using HIV.Interfaces;
using HIV.Mappers;
using HIV.Models;
using Microsoft.EntityFrameworkCore;

namespace HIV.Repository
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _context;
        public AppointmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserTableDTO>> GetAllListDoctor()
        {
            return await _context.Users
                        .Include(u => u.Schedules)
                        .Where(u => u.Role == "Doctor")
                        .Select(u => u.ToUserTableDTO())
                        .ToListAsync();
        }

        public async Task<List<ScheduleSimpleDTO>> GetScheduleOfDoctor(int id_doctor)
        {
            return await _context.Schedules.Where(u => u.DoctorId == id_doctor && u.Status.Equals("ACTIVE")).Select(u => new ScheduleSimpleDTO
            {
                ScheduleId = u.ScheduleId,
                ScheduledTime = u.ScheduledTime,
                Room = u.Room,
            }).ToListAsync();
        }

        public async Task<CreateAppointmentDTO> CreateAppointment(CreateAppointmentDTO dto)
        {
            try
            {
                if (await _context.Schedules.FindAsync(dto.ScheduleId) == null)
                {
                    throw new ArgumentException("ScheduleID không tồn tại");
                }

                if (await _context.Users.FindAsync(dto.doctorId) == null)
                {
                    throw new ArgumentException("DoctorID không tồn tại");
                }

                if (await _context.Users.FindAsync(dto.PatientId) == null)
                {
                    throw new ArgumentException("PatientID không tồn tại");
                }

                var appoint = new Appointment
                {
                    PatientId = dto.PatientId,
                    ScheduleId = dto.ScheduleId,
                    DoctorId = dto.doctorId,
                    Note = dto.Note,
                    IsAnonymous = (bool)dto.IsAnonymous,
                    Status = "CONFIRMED", // Changed from COMPLETED to CONFIRMED as default
                    CreatedAt = DateTime.Now,
                    AppointmentDate = dto.AppointmentDate
                };
                _context.Appointments.Add(appoint);

                var sche = await _context.Schedules.FindAsync(appoint.ScheduleId);
                if (sche != null)
                {
                    sche.Status = "ACTIVE";
                }

                await _context.SaveChangesAsync();
                return dto;
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message ?? ex.Message;
                Console.WriteLine($"Database update error: {innerException}");
                throw new Exception($"Lỗi khi lưu lịch hẹn: {innerException}", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating appointment: {ex.Message}");
                throw;
            }
        }

        public async Task<UserTableDTO> GetInforUser(int id)
        {
            var user = await _context.Users
                .Where(u => u.UserId == id)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            var schedules = await _context.Appointments
                .Where(a => a.PatientId == id)
                .Include(a => a.Schedule)
                .Select(a => a.Schedule)
                .Select(s => new ScheduleSimpleDTO
                {
                    ScheduleId = s.ScheduleId,
                    ScheduledTime = s.ScheduledTime,
                    Room = s.Room,
                    Status = s.Status
                }).ToListAsync();

            return new UserTableDTO
            {
                UserId = user.UserId,
                AccountId = user.AccountId,
                FullName = user.FullName,
                Phone = user.Phone,
                Gender = user.Gender,
                Birthdate = user.Birthdate,
                Role = user.Role,
                Schedules = schedules
            };
        }

        public async Task<bool> CancelAppointment(int id)
        {
            var appoint = await _context.Appointments.FindAsync(id);
            if (appoint == null)
            {
                throw new ArgumentException("Appointment không tồn tại");
            }

            appoint.Status = "CANCELLED";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<AppointmentDTO>> GetAll()
        {
            var listAppointment = await _context.Appointments.Select(a => new AppointmentDTO
            {
                AppointmentId = a.AppointmentId,
                ScheduleId = a.ScheduleId,
                doctorId = a.DoctorId,
                PatientId = a.PatientId,
                Note = a.Note,
                AppoinmentType = a.AppoinmentType,
                Status = a.Status,
                IsAnonymous = a.IsAnonymous,
                AppointmentDate = a.AppointmentDate,
                CreatedAt = a.CreatedAt
            }).ToListAsync();

            return listAppointment;
        }

        public async Task<List<PatientOfDoctorDTO>> GetPatientsOfDoctor(int doctorId)
        {
            var doctor = await _context.Users
                .Where(u => u.UserId == doctorId && u.Role == "Doctor")
                .FirstOrDefaultAsync();

            if (doctor == null)
            {
                throw new ArgumentException("Bác sĩ không tồn tại");
            }

            var patients = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Schedule)
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new PatientOfDoctorDTO
                {
                    PatientId = a.PatientId,
                    FullName = a.IsAnonymous ? "Bệnh nhân ẩn danh" : a.Patient.FullName ?? "Chưa cập nhật",
                    AppointmentId = a.AppointmentId,
                    AppointmentDate = a.AppointmentDate,
                    Status = a.Status,
                    Note = a.Note ?? "",
                    AppoinmentType = a.AppoinmentType ?? "",
                    IsAnonymous = a.IsAnonymous,
                    ScheduledTime = a.Schedule.ScheduledTime,
                    Room = a.Schedule.Room ?? "Chưa xác định",
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return patients;
        }

        // NEW METHODS FOR STATUS MANAGEMENT

        public async Task<bool> UpdateAppointmentStatus(UpdateAppointmentStatusDTO dto)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(dto.AppointmentId);
                if (appointment == null)
                {
                    throw new ArgumentException("Appointment không tồn tại");
                }

                // Validate status transition
                //if (!IsValidStatusTransition(appointment.Status, dto.Status))
                //{
                //    throw new ArgumentException($"Không thể chuyển từ {appointment.Status} sang {dto.Status}");
                //}

                appointment.Status = dto.Status;
                if (!string.IsNullOrEmpty(dto.Note))
                {
                    appointment.Note = dto.Note;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"Lỗi khi cập nhật status: {innerException}", ex);
            }
        }

        public async Task<bool> ConfirmAppointment(int appointmentId, string? note = null)
        {
            var dto = new UpdateAppointmentStatusDTO
            {
                AppointmentId = appointmentId,
                Status = "CONFIRMED",
                Note = note
            };
            return await UpdateAppointmentStatus(dto);
        }

        public async Task<bool> CompleteAppointment(int appointmentId, string? note = null)
        {
            var dto = new UpdateAppointmentStatusDTO
            {
                AppointmentId = appointmentId,
                Status = "COMPLETED",
                Note = note
            };
            return await UpdateAppointmentStatus(dto);
        }

        public async Task<AppointmentDTO> GetAppointmentById(int appointmentId)
        {
            var appointment = await _context.Appointments
                .Where(a => a.AppointmentId == appointmentId)
                .Select(a => new AppointmentDTO
                {
                    AppointmentId = a.AppointmentId,
                    ScheduleId = a.ScheduleId,
                    doctorId = a.DoctorId,
                    PatientId = a.PatientId,
                    Note = a.Note,
                    AppoinmentType = a.AppoinmentType,
                    Status = a.Status,
                    IsAnonymous = a.IsAnonymous,
                    AppointmentDate = a.AppointmentDate,
                    CreatedAt = a.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (appointment == null)
            {
                throw new ArgumentException("Appointment không tồn tại");
            }

            return appointment;
        }

        // Helper method to validate status transitions
        //private bool IsValidStatusTransition(string currentStatus, string newStatus)
        //{
        //    // Define valid transitions
        //    var validTransitions = new Dictionary<string, List<string>>
        //    {
        //        ["CONFIRMED"] = new List<string> { "COMPLETED", "CANCELLED" },
        //        ["COMPLETED"] = new List<string>(), // Cannot change from COMPLETED
        //        ["CANCELLED"] = new List<string>()  // Cannot change from CANCELLED
        //    };

        //    return validTransitions.ContainsKey(currentStatus) &&
        //           validTransitions[currentStatus].Contains(newStatus);
        //}
    }
}