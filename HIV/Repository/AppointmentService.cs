
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
                //status = u.Status
            }).ToListAsync();

        }

        public async Task<CreateAppointmentDTO> CreateAppointment(CreateAppointmentDTO dto)
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
                Status = "CONFIRMED",
                //Status = "Pending",
                CreatedAt = DateTime.Now,
                AppointmentDate = dto.AppointmentDate
            };
            _context.Appointments.Add(appoint);


            var sche = await _context.Schedules.FindAsync(appoint.ScheduleId);
            if (sche != null)
            {
                //sche.Status = "Booked";
                sche.Status = "ACTIVE";
            }
            await _context.SaveChangesAsync();
            return dto;
        }
        public async Task<UserTableDTO> GetInforUser(int id)
        {
            var user = await _context.Users.Where(u => u.UserId == id).FirstOrDefaultAsync();
            return new UserTableDTO
            {
                UserId = user.UserId,
                AccountId = user.AccountId,
                FullName = user.FullName,
                Phone = user.Phone,
                Gender = user.Gender,
                Birthdate = user.Birthdate,
                Role = user.Role
            };
        }
        public async Task<bool> CancelAppointment(int id)
        {
            var appoint = await _context.Appointments.FindAsync(id);
            if (appoint == null)
            {
                throw new ArgumentException("Appointment không tồn tại");
            }

            appoint.Status = "Cancel";
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
                Note = a.Note,
                AppoinmentType = a.AppoinmentType,
                Status = a.Status,
                IsAnonymous = a.IsAnonymous,
                AppointmentDate = a.AppointmentDate,
                CreatedAt = a.CreatedAt
            }).ToListAsync();

            return listAppointment;
        }
    }

}