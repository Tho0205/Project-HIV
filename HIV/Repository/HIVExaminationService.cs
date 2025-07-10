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
        private readonly IMedicalRecordService _medicalRecordService;

        public HIVExaminationService(AppDbContext context, ILogger<HIVExaminationService> logger, IMedicalRecordService medicalRecordService)
        {
            _context = context;
            _logger = logger;
            _medicalRecordService = medicalRecordService;
        }

        public async Task<List<PatientListDTO>> GetPatientsAsync(int page, int pageSize)
        {
            return await _context.Users
                .Include(u => u.Account)
                .Where(u => u.Role == "PATIENT" && u.Status == "ACTIVE")
                .Select(u => new PatientListDTO
                {
                    UserId = u.UserId,
                    FullName = u.FullName ?? "N/A",
                    Email = u.Account.Email ?? "N/A",
                    Phone = u.Phone,
                    Birthdate = u.Birthdate,
                    Gender = u.Gender,
                    Address = u.Address,
                    ExamCount = u.ExaminationsAsPatient.Count(e => e.Status == "ACTIVE"),
                    LastExamDate = u.ExaminationsAsPatient
                        .Where(e => e.Status == "ACTIVE")
                        .OrderByDescending(e => e.ExamDate)
                        .Select(e => e.ExamDate)
                        .FirstOrDefault()
                })
                .OrderBy(p => p.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<ExaminationDTO>> GetPatientExaminationsAsync(int patientId)
        {
            return await _context.Examinations
                .Include(e => e.Doctor)
                .Where(e => e.PatientId == patientId && e.Status == "ACTIVE")
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => new ExaminationDTO
                {
                    ExamId = e.ExamId,
                    DoctorName = e.Doctor.FullName ?? "N/A",
                    ExamDate = e.ExamDate ?? DateOnly.MinValue,
                    Result = e.Result ?? "",
                    Cd4Count = e.Cd4Count,
                    HivLoad = e.HivLoad,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = null // Add UpdatedAt field to Examination model if needed
                })
                .ToListAsync();
        }

        public async Task<ExaminationDTO> SaveExaminationAsync(ExaminationFormDTO dto)
        {
            Examination examination;

            if (dto.ExamId.HasValue)
            {
                // Update existing
                examination = await _context.Examinations
                    .FirstOrDefaultAsync(e => e.ExamId == dto.ExamId.Value && e.Status == "ACTIVE")
                    ?? throw new ArgumentException("Examination not found");

                // Only update allowed fields
                examination.DoctorId = dto.DoctorId;
                examination.ExamDate = dto.ExamDate;
                examination.Result = dto.Result;
                examination.Cd4Count = dto.Cd4Count;
                examination.HivLoad = dto.HivLoad;
                // Update timestamp if you have UpdatedAt field
            }
            else
            {
                // Create new
                examination = new Examination
                {
                    PatientId = dto.PatientId,
                    DoctorId = dto.DoctorId,
                    ExamDate = dto.ExamDate,
                    Result = dto.Result,
                    Cd4Count = dto.Cd4Count,
                    HivLoad = dto.HivLoad,
                    Status = "ACTIVE",
                    CreatedAt = DateTime.UtcNow
                };
                _context.Examinations.Add(examination);
            }

            await _context.SaveChangesAsync();

            // Return the saved examination
            return await _context.Examinations
                .Include(e => e.Doctor)
                .Where(e => e.ExamId == examination.ExamId)
                .Select(e => new ExaminationDTO
                {
                    ExamId = e.ExamId,
                    DoctorName = e.Doctor.FullName ?? "N/A",
                    ExamDate = e.ExamDate ?? DateOnly.MinValue,
                    Result = e.Result ?? "",
                    Cd4Count = e.Cd4Count,
                    HivLoad = e.HivLoad,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = null
                })
                .FirstAsync();
        }

        public async Task<bool> DeleteExaminationAsync(int examId)
        {
            var examination = await _context.Examinations
                .FirstOrDefaultAsync(e => e.ExamId == examId && e.Status == "ACTIVE");

            if (examination == null)
                return false;

            examination.Status = "PASSIVE";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<object>> GetDoctorsAsync()
        {
            return await _context.Users
                .Include(u => u.DoctorInfo)
                .Where(u => u.Role == "DOCTOR" && u.Status == "ACTIVE")
                .OrderBy(u => u.FullName)
                .Select(u => new
                {
                    value = u.UserId,
                    label = u.DoctorInfo != null && !string.IsNullOrEmpty(u.DoctorInfo.Degree)
                        ? $"{u.DoctorInfo.Degree} {u.FullName}"
                        : u.FullName
                })
                .ToListAsync<object>();
        }

        public async Task<bool> UpdateExamAsync(int examId, HIVExaminationDto dto)
        {
            var exam = await _context.Examinations.FindAsync(examId);
            if (exam == null) return false;

            exam.Cd4Count = dto.CD4Count;
            exam.HivLoad = dto.ViralLoad;

            await _context.SaveChangesAsync();

            return true;
        }

    }
}