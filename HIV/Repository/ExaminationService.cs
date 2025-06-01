
using HIV.Data;
using HIV.DTOs;
using HIV.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoSWP391.Services
{
    public class ExaminationService : IExaminationService
    {
        private readonly AppDbContext _context;

        public ExaminationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ExaminationResultDto>> GetPatientExaminationResultsAsync(int patientId)
        {
            var examinations = await _context.Examinations
                .Include(e => e.Doctor)
                .Include(e => e.Prescription)
                    .ThenInclude(p => p.CustomProtocol)
                        .ThenInclude(cp => cp.CustomizedArvProtocolDetails)
                            .ThenInclude(d => d.Arv)
                .Where(e => e.PatientId == patientId)
                .OrderByDescending(e => e.ExamDate)
                .Select(e => new ExaminationResultDto
                {
                    ExamId = e.ExamId,
                    DoctorName = e.Doctor != null ? e.Doctor.FullName : "Unknown",
                    Result = e.Result,
                    Cd4Count = e.Cd4Count,
                    HivLoad = e.HivLoad,
                    ExamDate = e.ExamDate,
                    PrescriptionDetails = e.Prescription != null && e.Prescription.CustomProtocol != null
                        ? string.Join(", ", e.Prescription.CustomProtocol.CustomizedArvProtocolDetails
                            .Select(d => $"{d.Arv.Name} - {d.Dosage}"))
                        : null
                })
                .ToListAsync();

            return examinations;
        }

        public async Task<ExaminationResultDto?> GetExaminationByIdAsync(int examId)
        {
            var examination = await _context.Examinations
                .Include(e => e.Doctor)
                .Include(e => e.Prescription)
                    .ThenInclude(p => p.CustomProtocol)
                        .ThenInclude(cp => cp.CustomizedArvProtocolDetails)
                            .ThenInclude(d => d.Arv)
                .Where(e => e.ExamId == examId)
                .Select(e => new ExaminationResultDto
                {
                    ExamId = e.ExamId,
                    DoctorName = e.Doctor != null ? e.Doctor.FullName : "Unknown",
                    Result = e.Result,
                    Cd4Count = e.Cd4Count,
                    HivLoad = e.HivLoad,
                    ExamDate = e.ExamDate,
                    PrescriptionDetails = e.Prescription != null && e.Prescription.CustomProtocol != null
                        ? string.Join(", ", e.Prescription.CustomProtocol.CustomizedArvProtocolDetails
                            .Select(d => $"{d.Arv.Name} - {d.Dosage}"))
                        : null
                })
                .FirstOrDefaultAsync();

            return examination;
        }

        public async Task<ExaminationResultDto> CreateExaminationAsync(CreateExaminationDto dto)
        {
            var examination = new Examination
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                Result = dto.Result,
                Cd4Count = dto.Cd4Count,
                HivLoad = dto.HivLoad,
                ExamDate = dto.ExamDate
            };

            _context.Examinations.Add(examination);
            await _context.SaveChangesAsync();

            return await GetExaminationByIdAsync(examination.ExamId)
                ?? throw new InvalidOperationException("Failed to retrieve created examination");
        }

        public async Task<ExaminationResultDto?> UpdateExaminationAsync(int examId, UpdateExaminationDto dto)
        {
            var examination = await _context.Examinations.FindAsync(examId);
            if (examination == null)
                return null;

            examination.Result = dto.Result ?? examination.Result;
            examination.Cd4Count = dto.Cd4Count ?? examination.Cd4Count;
            examination.HivLoad = dto.HivLoad ?? examination.HivLoad;

            await _context.SaveChangesAsync();

            return await GetExaminationByIdAsync(examId);
        }

        public async Task<bool> DeleteExaminationAsync(int examId)
        {
            var examination = await _context.Examinations.FindAsync(examId);
            if (examination == null)
                return false;

            _context.Examinations.Remove(examination);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}