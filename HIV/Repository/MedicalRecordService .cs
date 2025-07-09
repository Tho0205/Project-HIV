using HIV.DTOs;
using HIV.Interfaces;
using HIV.Models;
using Microsoft.EntityFrameworkCore;

namespace HIV.Repository
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly AppDbContext _context;

        public MedicalRecordService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MedicalRecordDto>> GetAllAsync()
        {
            return await _context.MedicalRecords
                .Include(m => m.Patient)
                .Include(m => m.Doctor)
                .Select(m => new MedicalRecordDto
                {
                    RecordId = m.RecordId,
                    PatientId = m.PatientId,
                    DoctorId = m.DoctorId,
                    ExamId = m.ExamId,
                    CustomProtocolId = m.CustomProtocolId,
                    ExamDate = m.ExamDate,
                    ExamTime = m.ExamTime,
                    Summary = m.Summary,
                    Status = m.Status,
                    IssuedAt = m.IssuedAt,
                    DoctorName = m.Doctor.FullName,
                    PatientName = m.Patient.FullName
                })
                .ToListAsync();
        }

        public async Task<MedicalRecordDto?> GetByIdAsync(int id)
        {
            var m = await _context.MedicalRecords
                .Include(x => x.Doctor)
                .Include(x => x.Patient)
                .FirstOrDefaultAsync(x => x.RecordId == id);

            if (m == null) return null;

            return new MedicalRecordDto
            {
                RecordId = m.RecordId,
                PatientId = m.PatientId,
                DoctorId = m.DoctorId,
                ExamId = m.ExamId,
                CustomProtocolId = m.CustomProtocolId,
                ExamDate = m.ExamDate,
                ExamTime = m.ExamTime,
                Summary = m.Summary,
                Status = m.Status,
                IssuedAt = m.IssuedAt,
                DoctorName = m.Doctor.FullName,
                PatientName = m.Patient.FullName
            };
        }

        public async Task<MedicalRecordDto> CreateAsync(CreateMedicalRecordDto dto)
        {
            var entity = new MedicalRecord
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                ExamId = dto.ExamId,
                CustomProtocolId = dto.CustomProtocolId,
                ExamDate = dto.ExamDate,
                ExamTime = dto.ExamTime,
                Summary = dto.Summary,
                IssuedAt = DateTime.UtcNow,
                Status = "ACTIVE"
            };

            _context.MedicalRecords.Add(entity);
            await _context.SaveChangesAsync();

            return new MedicalRecordDto
            {
                RecordId = entity.RecordId,
                PatientId = entity.PatientId,
                DoctorId = entity.DoctorId,
                ExamId = entity.ExamId,
                CustomProtocolId = entity.CustomProtocolId,
                ExamDate = entity.ExamDate,
                ExamTime = entity.ExamTime,
                Summary = entity.Summary,
                Status = entity.Status,
                IssuedAt = entity.IssuedAt
            };
        }

        public async Task<IEnumerable<MedicalRecordDto>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.MedicalRecords
                .Include(m => m.Patient)
                .Include(m => m.Doctor)
                .Where(m => m.DoctorId == doctorId && m.Status != "DELETED")
                .Select(m => new MedicalRecordDto
                {
                    RecordId = m.RecordId,
                    PatientId = m.PatientId,
                    DoctorId = m.DoctorId,
                    ExamId = m.ExamId,
                    CustomProtocolId = m.CustomProtocolId,
                    ExamDate = m.ExamDate,
                    ExamTime = m.ExamTime,
                    Summary = m.Summary,
                    Status = m.Status,
                    IssuedAt = m.IssuedAt,
                    DoctorName = m.Doctor.FullName,
                    PatientName = m.Patient.FullName
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicalRecordDto>> GetByPatientIdAsync(int patientId)
        {
            return await _context.MedicalRecords
                .Include(m => m.Patient)
                .Include(m => m.Doctor)
                .Where(m => m.PatientId == patientId && m.Status != "DELETED")
                .Select(m => new MedicalRecordDto
                {
                    RecordId = m.RecordId,
                    PatientId = m.PatientId,
                    DoctorId = m.DoctorId,
                    ExamId = m.ExamId,
                    CustomProtocolId = m.CustomProtocolId,
                    ExamDate = m.ExamDate,
                    ExamTime = m.ExamTime,
                    Summary = m.Summary,
                    Status = m.Status,
                    IssuedAt = m.IssuedAt,
                    DoctorName = m.Doctor.FullName,
                    PatientName = m.Patient.FullName
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(int id, UpdateMedicalRecordDto dto)
        {
            var entity = await _context.MedicalRecords.FindAsync(id);
            if (entity == null) return false;

            entity.ExamDate = dto.ExamDate;
            entity.ExamTime = dto.ExamTime;
            entity.Summary = dto.Summary;
            entity.Status = dto.Status;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.MedicalRecords.FindAsync(id);
            if (entity == null) return false;

            entity.Status = "DELETED";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
