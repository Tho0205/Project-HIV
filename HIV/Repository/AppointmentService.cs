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
        private readonly INotificationService _notificationService;

        public AppointmentService(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
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
                Status = u.Status
            }).ToListAsync();
        }
        public async Task<List<ScheduleSimpleDTO>> GetAllScheduleOfDoctor(int id_doctor)
        {
            return await _context.Schedules.Where(u => u.DoctorId == id_doctor).Select(u => new ScheduleSimpleDTO
            {
                ScheduleId = u.ScheduleId,
                ScheduledTime = u.ScheduledTime,
                Room = u.Room,
                Status = u.Status
            }).ToListAsync();
        }
        public async Task CreateRelatedRecordsAsync(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
            {
                Console.WriteLine($"❌ Appointment {appointmentId} not found");
                return;
            }

            Console.WriteLine($"🚀 Creating related records for Appointment {appointmentId}");

            try
            {
                // Creating Examination (đã OK)
                Console.WriteLine($"🏥 Creating Examination...");
                var examination = new Examination
                {
                    PatientId = appointment.PatientId,
                    DoctorId = appointment.DoctorId,
                    AppointmentId = appointmentId,  // ✅ Đã có
                    ExamDate = DateOnly.FromDateTime(appointment.AppointmentDate),
                    Result = "Scheduled for examination",
                    Cd4Count = null,
                    HivLoad = null,
                    Status = "ACTIVE",
                    CreatedAt = DateTime.Now
                };

                _context.Examinations.Add(examination);
                await _context.SaveChangesAsync();
                Console.WriteLine($"✅ Examination created successfully");

                // ✅ SỬA PHẦN TạO CustomizedArvProtocol - THÊM AppointmentId
                Console.WriteLine($"💊 Creating CustomizedArvProtocol...");

                var countBefore = await _context.Database
                    .SqlQueryRaw<int>("SELECT COUNT(*) as Value FROM CustomizedARV_Protocol")
                    .FirstOrDefaultAsync();
                Console.WriteLine($"📊 Records before insert: {countBefore}");

                Console.WriteLine($"📝 Values to insert:");
                Console.WriteLine($"   DoctorId: {appointment.DoctorId}");
                Console.WriteLine($"   PatientId: {appointment.PatientId}");
                Console.WriteLine($"   AppointmentId: {appointmentId}");  // ✅ THÊM LOG
                Console.WriteLine($"   BaseProtocolId: NULL");
                Console.WriteLine($"   Name: Protocol for Appointment #{appointmentId}");
                Console.WriteLine($"   Description: Auto-generated from appointment booking");
                Console.WriteLine($"   Status: ACTIVE");

                // ✅ SỬA SQL - THÊM AppointmentId VÀO
                var protocolSql = @"
            INSERT INTO CustomizedARV_Protocol (DoctorId, PatientId, AppointmentId, BaseProtocolId, Name, Description, Status)
            VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})";

                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(protocolSql,
                    appointment.DoctorId,
                    appointment.PatientId,
                    appointmentId,              // ✅ THÊM PARAMETER NÀY
                    DBNull.Value,               // BaseProtocolId
                    $"Protocol for Appointment #{appointmentId}",
                    "Auto-generated from appointment booking",
                    "ACTIVE");

                Console.WriteLine($"📊 SQL ExecuteSqlRawAsync returned: {rowsAffected} rows affected");

                var countAfter = await _context.Database
                    .SqlQueryRaw<int>("SELECT COUNT(*) as Value FROM CustomizedARV_Protocol")
                    .FirstOrDefaultAsync();
                Console.WriteLine($"📊 Records after insert: {countAfter}");

                if (countAfter > countBefore)
                {
                    Console.WriteLine($"✅ SUCCESS: {countAfter - countBefore} new record(s) inserted!");

                    var newRecord = await _context.Database
                        .SqlQueryRaw<int>($"SELECT TOP 1 CustomProtocolId as Value FROM CustomizedARV_Protocol WHERE Name = 'Protocol for Appointment #{appointmentId}' ORDER BY CustomProtocolId DESC")
                        .FirstOrDefaultAsync();
                    Console.WriteLine($"🆕 New record ID: {newRecord}");
                }
                else
                {
                    Console.WriteLine($"⚠️ WARNING: No new records detected despite rowsAffected = {rowsAffected}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Protocol creation failed: {ex.Message}");

                // ✅ SỬA FALLBACK SQL - THÊM AppointmentId
                try
                {
                    Console.WriteLine($"🔄 Trying minimal insert...");

                    var minimalSql = @"
                INSERT INTO CustomizedARV_Protocol (DoctorId, PatientId, AppointmentId, Name, Status)
                VALUES ({0}, {1}, {2}, {3}, {4})";

                    var minimalRows = await _context.Database.ExecuteSqlRawAsync(minimalSql,
                        appointment.DoctorId,
                        appointment.PatientId,
                        appointmentId,          // ✅ THÊM PARAMETER NÀY
                        $"Test Protocol {appointmentId}",
                        "ACTIVE");

                    Console.WriteLine($"✅ Minimal insert successful: {minimalRows} rows");
                }
                catch (Exception minEx)
                {
                    Console.WriteLine($"❌ Minimal insert failed: {minEx.Message}");
                }
            }

            Console.WriteLine($"🎉 CreateRelatedRecordsAsync completed for Appointment {appointmentId}");
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
                    Status = "SCHEDULED",
                    CreatedAt = DateTime.Now,
                    AppointmentDate = dto.AppointmentDate
                };
                _context.Appointments.Add(appoint);

                var sche = await _context.Schedules.FindAsync(appoint.ScheduleId);
                if (sche != null)
                {
                    sche.Status = "INACTIVE";
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
            var sche = await _context.Schedules.FindAsync(appoint.ScheduleId);
            if (sche != null)
            {
                sche.Status = "ACTIVE";
            }
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

        public async Task<bool> UpdateAppointmentStatus(UpdateAppointmentStatusDTO dto)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(dto.AppointmentId);
                if (appointment == null)
                {
                    throw new ArgumentException("Appointment không tồn tại");
                }
                appointment.Status = dto.Status;
                if (appointment.Status == "CONFIRMED")
                {
                    if (appointment.PatientId != null)
                    {

                        await _notificationService.CreateMedicationReminders(appointment.PatientId);
                    }
                }
                if(appointment.Status == "CHECKED_IN" && appointment.IsAnonymous == false)
                {
                    await _notificationService.CreateAppointmentReminders(appointment.AppointmentId);
                    await CreateRelatedRecordsAsync(appointment.AppointmentId);

                }
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

            var result = await UpdateAppointmentStatus(dto);

            //if (result)
            //{
            //    var appointment = await _context.Appointments.FindAsync(appointmentId);
            //    if (appointment?.PatientId != null)
            //    {
            //        // Tạo thông báo nhắc uống thuốc
            //        await _notificationService.CreateMedicationReminders(appointment.PatientId);
            //    }
            //}

            return result;
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
    }
}