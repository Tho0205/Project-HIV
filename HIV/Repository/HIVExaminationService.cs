using HIV.DTOs;
using HIV.Interfaces;
using HIV.Models;
using Microsoft.EntityFrameworkCore;

namespace HIV.Repository
{
    public class HIVExaminationService : IHIVExaminationService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HIVExaminationService> _logger;

        public HIVExaminationService(AppDbContext context, ILogger<HIVExaminationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<HIVExaminationResponseDTO> CreateHIVExaminationAsync(CreateHIVExaminationDTO createDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Bắt đầu tạo kết quả xét nghiệm HIV cho bệnh nhân {PatientId}", createDto.PatientId);

                // 1. Validate bệnh nhân và bác sĩ tồn tại
                await ValidatePatientAndDoctorAsync(createDto.PatientId, createDto.DoctorId);

                // 2. Tạo Examination record
                var examination = new Examination
                {
                    PatientId = createDto.PatientId,
                    DoctorId = createDto.DoctorId,
                    ExamDate = createDto.ExamDate,
                    Result = createDto.Result,
                    Cd4Count = createDto.Cd4Count,
                    HivLoad = createDto.HivLoad,
                    Status = "ACTIVE",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Examinations.Add(examination);
                await _context.SaveChangesAsync();

                // 3. Tạo MedicalRecord nếu có CurrentCondition
                if (!string.IsNullOrEmpty(createDto.CurrentCondition))
                {
                    var medicalRecord = new MedicalRecord
                    {
                        PatientId = createDto.PatientId,
                        DoctorId = createDto.DoctorId,
                        ExamId = examination.ExamId,
                        ExamDate = createDto.ExamDate.ToDateTime(TimeOnly.MinValue),
                        ExamTime = DateTime.Now.TimeOfDay,
                        Summary = createDto.CurrentCondition,
                        Status = "ACTIVE",
                        IssuedAt = DateTime.UtcNow
                    };

                    _context.MedicalRecords.Add(medicalRecord);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                _logger.LogInformation("Tạo kết quả xét nghiệm HIV thành công - ExamId: {ExamId}", examination.ExamId);

                // 4. Trả về response
                return await GetHIVExaminationByIdAsync(examination.ExamId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi tạo kết quả xét nghiệm HIV cho bệnh nhân {PatientId}", createDto.PatientId);
                throw;
            }
        }

        public async Task<HIVExaminationResponseDTO> UpdateHIVExaminationAsync(int examId, UpdateHIVExaminationDTO updateDto)
        {
            try
            {
                var examination = await _context.Examinations
                    .FirstOrDefaultAsync(e => e.ExamId == examId && e.Status != "DELETED");

                if (examination == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy kết quả xét nghiệm");
                }

                // Update examination
                if (!string.IsNullOrEmpty(updateDto.Result))
                    examination.Result = updateDto.Result;

                if (updateDto.Cd4Count.HasValue)
                    examination.Cd4Count = updateDto.Cd4Count;

                if (updateDto.HivLoad.HasValue)
                    examination.HivLoad = updateDto.HivLoad;

                // Update medical record nếu có
                if (!string.IsNullOrEmpty(updateDto.Summary))
                {
                    var medicalRecord = await _context.MedicalRecords
                        .FirstOrDefaultAsync(mr => mr.ExamId == examId);

                    if (medicalRecord != null)
                    {
                        medicalRecord.Summary = updateDto.Summary;
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Cập nhật kết quả xét nghiệm HIV thành công - ExamId: {ExamId}", examId);

                return await GetHIVExaminationByIdAsync(examId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật kết quả xét nghiệm HIV - ExamId: {ExamId}", examId);
                throw;
            }
        }

        public async Task<HIVExaminationResponseDTO> GetHIVExaminationByIdAsync(int examId)
        {
            var examination = await _context.Examinations
                .Include(e => e.Patient)
                    .ThenInclude(p => p.Account)
                .Include(e => e.Doctor)
                .Include(e => e.MedicalRecord)
                .FirstOrDefaultAsync(e => e.ExamId == examId && e.Status != "DELETED");

            if (examination == null)
            {
                throw new KeyNotFoundException("Không tìm thấy kết quả xét nghiệm");
            }

            return new HIVExaminationResponseDTO
            {
                ExamId = examination.ExamId,
                PatientId = examination.PatientId,
                PatientName = examination.Patient.FullName ?? "N/A",
                PatientEmail = examination.Patient.Account?.Email ?? "N/A",
                DoctorId = examination.DoctorId,
                DoctorName = examination.Doctor.FullName ?? "N/A",
                ExamDate = examination.ExamDate ?? DateOnly.MinValue,
                Result = examination.Result ?? "",
                Cd4Count = examination.Cd4Count,
                HivLoad = examination.HivLoad,
                Status = examination.Status,
                CreatedAt = examination.CreatedAt,
                MedicalRecordSummary = examination.MedicalRecord?.Summary
            };
        }

        public async Task<List<HIVExaminationResponseDTO>> GetPatientHIVExaminationHistoryAsync(int patientId)
        {
            var examinations = await _context.Examinations
                .Include(e => e.Patient)
                    .ThenInclude(p => p.Account)
                .Include(e => e.Doctor)
                .Include(e => e.MedicalRecord)
                .Where(e => e.PatientId == patientId && e.Status != "DELETED")
                .OrderByDescending(e => e.ExamDate)
                .ToListAsync();

            return examinations.Select(e => new HIVExaminationResponseDTO
            {
                ExamId = e.ExamId,
                PatientId = e.PatientId,
                PatientName = e.Patient.FullName ?? "N/A",
                PatientEmail = e.Patient.Account?.Email ?? "N/A",
                DoctorId = e.DoctorId,
                DoctorName = e.Doctor.FullName ?? "N/A",
                ExamDate = e.ExamDate ?? DateOnly.MinValue,
                Result = e.Result ?? "",
                Cd4Count = e.Cd4Count,
                HivLoad = e.HivLoad,
                Status = e.Status,
                CreatedAt = e.CreatedAt,
                MedicalRecordSummary = e.MedicalRecord?.Summary
            }).ToList();
        }

        public async Task<List<HIVExaminationSummaryDTO>> GetAllHIVExaminationsAsync(int page, int pageSize)
        {
            var examinations = await _context.Examinations
                .Include(e => e.Patient)
                .Include(e => e.Doctor)
                .Where(e => e.Status != "DELETED")
                .OrderByDescending(e => e.ExamDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return examinations.Select(e => new HIVExaminationSummaryDTO
            {
                ExamId = e.ExamId,
                PatientName = e.Patient.FullName ?? "N/A",
                DoctorName = e.Doctor.FullName ?? "N/A",
                ExamDate = e.ExamDate ?? DateOnly.MinValue,
                Cd4Count = e.Cd4Count,
                HivLoad = e.HivLoad,
                ResultSummary = e.Result?.Length > 50 ? e.Result.Substring(0, 50) + "..." : e.Result ?? "",
                Status = e.Status
            }).ToList();
        }

        public async Task<List<DoctorSimpleDTO>> GetDoctorsForSelectionAsync()
        {
            var doctors = await _context.Users
                .Include(u => u.DoctorInfo)
                .Where(u => u.Role == "Doctor" && u.Status == "ACTIVE")
                .OrderBy(u => u.FullName)
                .ToListAsync();

            return doctors.Select(d => new DoctorSimpleDTO
            {
                UserId = d.UserId,
                FullName = d.FullName ?? "N/A",
                Specialization = d.DoctorInfo?.Specialization,
                Degree = d.DoctorInfo?.Degree
            }).ToList();
        }

        public async Task<bool> DeleteHIVExaminationAsync(int examId)
        {
            var examination = await _context.Examinations
                .FirstOrDefaultAsync(e => e.ExamId == examId);

            if (examination == null)
                return false;

            examination.Status = "DELETED";
            await _context.SaveChangesAsync();

            _logger.LogInformation("Xóa kết quả xét nghiệm HIV - ExamId: {ExamId}", examId);
            return true;
        }

        public async Task<bool> CanAccessExaminationAsync(int examId, int userId)
        {
            return await _context.Examinations
                .AnyAsync(e => e.ExamId == examId &&
                          (e.PatientId == userId || e.DoctorId == userId) &&
                          e.Status != "DELETED");
        }

        // Private helper methods
        private async Task ValidatePatientAndDoctorAsync(int patientId, int doctorId)
        {
            var patient = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == patientId && u.Role == "Patient" && u.Status == "ACTIVE");

            if (patient == null)
            {
                throw new ArgumentException("Không tìm thấy bệnh nhân hoặc bệnh nhân không hoạt động");
            }

            var doctor = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == doctorId && u.Role == "Doctor" && u.Status == "ACTIVE");

            if (doctor == null)
            {
                throw new ArgumentException("Không tìm thấy bác sĩ hoặc bác sĩ không hoạt động");
            }
        }
    }
}