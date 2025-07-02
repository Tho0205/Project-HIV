using HIV.DTOs.DoctorPatient;
using HIV.Interfaces;
using HIV.Models;
using Microsoft.EntityFrameworkCore;

namespace HIV.Repository
{
    public class DoctorPatientService : IDoctorMangamentPatient
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DoctorPatientService> _logger;

        public DoctorPatientService(AppDbContext context, ILogger<DoctorPatientService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DoctorPatientsResponseDto> GetDoctorPatientsAsync(
            int doctorId,
            string sortBy = "full_name",
            string order = "asc",
            int page = 1,
            int pageSize = 8)
        {
            try
            {
                // Get User from doctorId
                var doctorUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == doctorId);

                if (doctorUser == null)
                {
                    return new DoctorPatientsResponseDto
                    {
                        Total = 0,
                        Page = page,
                        PageSize = pageSize,
                        Data = new List<DoctorPatientListDto>(),
                        Stats = new DoctorPatientStatsDto()
                    };
                }

                // Lấy danh sách patient IDs từ appointments
                var patientIds = await _context.Appointments
                    .Where(a => a.DoctorId == doctorUser.UserId)
                    .Select(a => a.PatientId)
                    .Distinct()
                    .ToListAsync();

                if (!patientIds.Any())
                {
                    return new DoctorPatientsResponseDto
                    {
                        Total = 0,
                        Page = page,
                        PageSize = pageSize,
                        Data = new List<DoctorPatientListDto>(),
                        Stats = await GetDoctorPatientStatsAsync(doctorId)
                    };
                }

                var query = _context.Accounts
                    .Include(a => a.User)
                    .Where(a => a.User.Role == "Patient" &&
                               patientIds.Contains(a.User.UserId) &&
                               a.User.Status != "DELETED")
                    .AsQueryable();

                // Áp dụng sắp xếp
                query = ApplySorting(query, sortBy, order);

                var totalCount = await query.CountAsync();

                var patients = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(account => new DoctorPatientListDto
                    {
                        AccountId = account.AccountId,
                        UserId = account.User.UserId,
                        Email = account.Email ?? "",
                        CreatedAt = account.CreatedAt,
                        FullName = account.User.FullName ?? "",
                        Phone = account.User.Phone,
                        Gender = account.User.Gender ?? "Other",
                        Birthdate = account.User.Birthdate,
                        Address = account.User.Address ?? "",
                        UserAvatar = account.User.UserAvatar,
                        Status = account.User.Status,
                        AppointmentCount = _context.Appointments
                            .Count(app => app.PatientId == account.User.UserId && app.DoctorId == doctorUser.UserId),
                        LastAppointmentDate = _context.Appointments
                            .Where(app => app.PatientId == account.User.UserId && app.DoctorId == doctorUser.UserId)
                            .OrderByDescending(app => app.AppointmentDate)
                            .Select(app => app.AppointmentDate)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                return new DoctorPatientsResponseDto
                {
                    Total = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    Data = patients,
                    Stats = await GetDoctorPatientStatsAsync(doctorId)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor patients for doctorId: {DoctorId}", doctorId);
                throw new ApplicationException("Không thể tải danh sách bệnh nhân", ex);
            }
        }

        public async Task<List<DoctorPatientListDto>> GetAvailablePatientsAsync()
        {
            try
            {
                // Lấy tất cả patient IDs đã có appointment
                var assignedPatientIds = await _context.Appointments
                    .Select(a => a.PatientId)
                    .Distinct()
                    .ToListAsync();

                // Lấy patients chưa có appointment nào
                var availablePatients = await _context.Accounts
                    .Include(a => a.User)
                    .Where(a => a.User.Role == "Patient" &&
                               a.User.Status == "ACTIVE" &&
                               !assignedPatientIds.Contains(a.User.UserId))
                    .Select(account => new DoctorPatientListDto
                    {
                        AccountId = account.AccountId,
                        UserId = account.User.UserId,
                        Email = account.Email ?? "",
                        CreatedAt = account.CreatedAt,
                        FullName = account.User.FullName ?? "",
                        Phone = account.User.Phone,
                        Gender = account.User.Gender ?? "Other",
                        Birthdate = account.User.Birthdate,
                        Address = account.User.Address ?? "",
                        UserAvatar = account.User.UserAvatar,
                        Status = account.User.Status,
                        AppointmentCount = 0,
                        LastAppointmentDate = null
                    })
                    .OrderBy(p => p.FullName)
                    .ToListAsync();

                return availablePatients;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available patients");
                throw new ApplicationException("Không thể tải danh sách bệnh nhân khả dụng", ex);
            }
        }

        public async Task<bool> AssignPatientToDoctorAsync(int doctorId, int patientId)
        {
            try
            {
                // Lấy User từ accountId hoặc userId
                var patientUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.AccountId == patientId || u.UserId == patientId);

                if (patientUser == null)
                {
                    _logger.LogWarning("Patient not found with id: {PatientId}", patientId);
                    return false;
                }

                // Kiểm tra patient chưa có appointment nào
                var hasAppointment = await _context.Appointments
                    .AnyAsync(a => a.PatientId == patientUser.UserId);

                if (hasAppointment)
                {
                    _logger.LogWarning("Patient {PatientId} already has appointments", patientUser.UserId);
                    return false;
                }

                // Lấy schedule có sẵn của doctor hoặc tạo mới
                var existingSchedule = await _context.Schedules
                    .Where(s => s.DoctorId == doctorId &&
                               s.Status == "ACTIVE" &&
                               s.ScheduledTime.Date >= DateTime.Now.Date)
                    .OrderBy(s => s.ScheduledTime)
                    .FirstOrDefaultAsync();

                int scheduleId;

                if (existingSchedule != null)
                {
                    scheduleId = existingSchedule.ScheduleId;
                }
                else
                {
                    // Tạo schedule mới nếu chưa có
                    var newSchedule = new Schedule
                    {
                        DoctorId = doctorId,
                        ScheduledTime = DateTime.Now.AddDays(7).Date.AddHours(9), // 9 giờ sáng
                        Room = $"P{doctorId}", // Phòng theo mã bác sĩ
                        Status = "ACTIVE"
                    };
                    _context.Schedules.Add(newSchedule);
                    await _context.SaveChangesAsync();
                    scheduleId = newSchedule.ScheduleId;
                }

                // Tạo appointment
                var appointment = new Appointment
                {
                    DoctorId = doctorId,
                    PatientId = patientUser.UserId,
                    ScheduleId = scheduleId,
                    AppointmentDate = DateTime.Now.AddDays(7),
                    Status = "SCHEDULED",
                    Note = "Đã được phân công cho bác sĩ",
                    CreatedAt = DateTime.Now
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Patient {PatientId} assigned to doctor {DoctorId}", patientUser.UserId, doctorId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning patient to doctor");
                return false;
            }
        }

        public async Task<DoctorPatientStatsDto> GetDoctorPatientStatsAsync(int doctorId)
        {
            try
            {
                var doctorUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == doctorId);

                if (doctorUser == null)
                {
                    return new DoctorPatientStatsDto
                    {
                        TotalPatients = 0,
                        TodayAppointments = 0,
                        ControlledPatients = 0,
                        UnstablePatients = 0
                    };
                }

                // 1. Lấy danh sách patient IDs
                var patientIds = await _context.Appointments
                    .Where(a => a.DoctorId == doctorUser.UserId)
                    .Select(a => a.PatientId)
                    .Distinct()
                    .ToListAsync();

                // 2. Tổng số bệnh nhân
                var totalPatients = patientIds.Count;

                // 3. Lịch hẹn hôm nay
                var today = DateTime.Now.Date;
                var todayAppointments = await _context.Appointments
                    .Where(a => a.DoctorId == doctorUser.UserId &&
                               a.AppointmentDate.Date == today &&
                               a.Status == "SCHEDULED") // Chỉ đếm lịch đã lên
                    .CountAsync();

                // 4. Phân loại bệnh nhân theo chỉ số HIV
                int controlledCount = 0;
                int unstableCount = 0;

                foreach (var patientId in patientIds)
                {
                    var latestExam = await _context.Examinations
                        .Where(e => e.PatientId == patientId &&
                                   e.DoctorId == doctorUser.UserId &&
                                   e.Status == "ACTIVE" &&
                                   e.Cd4Count.HasValue &&
                                   e.HivLoad.HasValue)
                        .OrderByDescending(e => e.ExamDate ?? DateOnly.FromDateTime(e.CreatedAt))
                        .FirstOrDefaultAsync();

                    if (latestExam != null)
                    {
                        // Đã kiểm soát: CD4 >= 500 HOẶC HIV Load < 200
                        if (latestExam.Cd4Count >= 500 || latestExam.HivLoad < 200)
                        {
                            controlledCount++;
                        }
                        // Bất ổn: CD4 < 500 HOẶC HIV Load >= 200
                        else if (latestExam.Cd4Count < 500 || latestExam.HivLoad >= 200)
                        {
                            unstableCount++;
                        }
                    }
                }

                return new DoctorPatientStatsDto
                {
                    TotalPatients = totalPatients,
                    TodayAppointments = todayAppointments,
                    ControlledPatients = controlledCount,
                    UnstablePatients = unstableCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor stats for doctorId: {DoctorId}", doctorId);
                throw new ApplicationException("Không thể tải thống kê", ex);
            }
        }

        public async Task<PatientHistoryDto> GetPatientHistoryAsync(int patientId, int doctorId)
        {
            try
            {
                // Get User từ doctorId
                var doctorUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == doctorId);

                // Tìm patient user từ accountId hoặc userId
                var patientUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == patientId || u.AccountId == patientId);

                if (doctorUser == null || patientUser == null)
                {
                    _logger.LogWarning("Doctor or patient not found. DoctorId: {DoctorId}, PatientId: {PatientId}",
                        doctorId, patientId);
                    return new PatientHistoryDto
                    {
                        Appointments = new List<AppointmentHistoryDto>(),
                        Examinations = new List<ExaminationHistoryDto>()
                    };
                }

                // Query với userId thực
                var appointments = await _context.Appointments
                    .Include(a => a.Schedule)
                    .Where(a => a.PatientId == patientUser.UserId &&
                               a.DoctorId == doctorUser.UserId)
                    .OrderByDescending(a => a.AppointmentDate)
                    .Select(a => new AppointmentHistoryDto
                    {
                        AppointmentId = a.AppointmentId,
                        AppointmentDate = a.AppointmentDate,
                        Status = a.Status,
                        Note = a.Note,
                        Room = a.Schedule != null ? a.Schedule.Room : null,
                        CreatedAt = a.CreatedAt
                    })
                    .ToListAsync();

                var examinations = await _context.Examinations
                    .Where(e => e.PatientId == patientUser.UserId && e.DoctorId == doctorUser.UserId && e.Status == "ACTIVE")
                    .OrderByDescending(e => e.ExamDate)
                    .Select(e => new ExaminationHistoryDto
                    {
                        ExamId = e.ExamId,
                        ExamDate = e.ExamDate,
                        Result = e.Result ?? "",
                        Cd4Count = e.Cd4Count,
                        HivLoad = e.HivLoad,
                        CreatedAt = e.CreatedAt
                    })
                    .ToListAsync();

                return new PatientHistoryDto
                {
                    Appointments = appointments,
                    Examinations = examinations
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient history for patientId: {PatientId}, doctorId: {DoctorId}", patientId, doctorId);
                throw new ApplicationException("Không thể tải lịch sử bệnh nhân", ex);
            }
        }

        public async Task<bool> CanDoctorAccessPatientAsync(int doctorId, int patientId)
        {
            try
            {
                // Get User từ doctorId
                var doctorUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == doctorId);

                // Kiểm tra cả accountId và userId cho patient
                var patientUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == patientId || u.AccountId == patientId);

                if (doctorUser == null || patientUser == null)
                {
                    _logger.LogWarning("Doctor or patient not found. DoctorId: {DoctorId}, PatientId: {PatientId}",
                        doctorId, patientId);
                    return false;
                }

                // Kiểm tra appointment với userId của patient
                var hasAccess = await _context.Appointments
                    .AnyAsync(a => a.DoctorId == doctorUser.UserId &&
                                  a.PatientId == patientUser.UserId);

                _logger.LogInformation("Access check - DoctorUserId: {DoctorUserId}, PatientUserId: {PatientUserId}, HasAccess: {HasAccess}",
                    doctorUser.UserId, patientUser.UserId, hasAccess);

                return hasAccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking doctor access for doctorId: {DoctorId}, patientId: {PatientId}",
                    doctorId, patientId);
                return false;
            }
        }

        public async Task<DoctorPatientListDto?> GetPatientDetailAsync(int patientId, int doctorId)
        {
            try
            {
                // Kiểm tra quyền truy cập
                if (!await CanDoctorAccessPatientAsync(doctorId, patientId))
                {
                    return null;
                }

                // Get User from doctorId
                var doctorUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == doctorId);

                var patientUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == patientId || u.AccountId == patientId);

                if (doctorUser == null || patientUser == null)
                {
                    return null;
                }

                return await _context.Accounts
                    .Include(a => a.User)
                    .Where(a => a.User.UserId == patientUser.UserId)
                    .Select(a => new DoctorPatientListDto
                    {
                        AccountId = a.AccountId,
                        Email = a.Email ?? "",
                        CreatedAt = a.CreatedAt,
                        FullName = a.User.FullName ?? "",
                        Phone = a.User.Phone,
                        Gender = a.User.Gender ?? "Other",
                        Birthdate = a.User.Birthdate,
                        Address = a.User.Address ?? "",
                        UserAvatar = a.User.UserAvatar,
                        Status = a.User.Status,
                        AppointmentCount = _context.Appointments
                            .Count(app => app.PatientId == patientUser.UserId && app.DoctorId == doctorUser.UserId),
                        LastAppointmentDate = _context.Appointments
                            .Where(app => app.PatientId == patientUser.UserId && app.DoctorId == doctorUser.UserId)
                            .OrderByDescending(app => app.AppointmentDate)
                            .Select(app => app.AppointmentDate)
                            .FirstOrDefault()
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient detail for patientId: {PatientId}, doctorId: {DoctorId}", patientId, doctorId);
                return null;
            }
        }

        private static IQueryable<Account> ApplySorting(IQueryable<Account> query, string sortBy, string order)
        {
            return (sortBy.ToLower(), order.ToLower()) switch
            {
                ("full_name", "asc") => query.OrderBy(a => a.User.FullName),
                ("full_name", "desc") => query.OrderByDescending(a => a.User.FullName),
                ("created_at", "asc") => query.OrderBy(a => a.CreatedAt),
                ("created_at", "desc") => query.OrderByDescending(a => a.CreatedAt),
                ("email", "asc") => query.OrderBy(a => a.Email),
                ("email", "desc") => query.OrderByDescending(a => a.Email),
                ("status", "asc") => query.OrderBy(a => a.User.Status),
                ("status", "desc") => query.OrderByDescending(a => a.User.Status),
                _ => query.OrderBy(a => a.User.FullName)
            };
        }
    }
}