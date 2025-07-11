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

        // Phương thức mới để lấy thông tin chi tiết
        public async Task<MedicalRecordDetailDto?> GetDetailByIdAsync(int id)
        {
            var medicalRecord = await _context.MedicalRecords
                .Include(m => m.Patient)
                .Include(m => m.Doctor)
                .Include(m => m.Examination)
                .Include(m => m.CustomProtocol)
                    .ThenInclude(cp => cp.BaseProtocol)
                .Include(m => m.CustomProtocol)
                    .ThenInclude(cp => cp.Details)
                        .ThenInclude(d => d.Arv)
                .FirstOrDefaultAsync(m => m.RecordId == id);

            if (medicalRecord == null) return null;

            var result = new MedicalRecordDetailDto
            {
                RecordId = medicalRecord.RecordId,
                PatientId = medicalRecord.PatientId,
                DoctorId = medicalRecord.DoctorId,
                ExamId = medicalRecord.ExamId,
                CustomProtocolId = medicalRecord.CustomProtocolId,
                ExamDate = medicalRecord.ExamDate,
                ExamTime = medicalRecord.ExamTime,
                Status = medicalRecord.Status,
                IssuedAt = medicalRecord.IssuedAt,
                Summary = medicalRecord.Summary,
                DoctorName = medicalRecord.Doctor?.FullName,
                PatientName = medicalRecord.Patient?.FullName
            };

            // Map thông tin Examination
            if (medicalRecord.Examination != null)
            {
                result.Examination = new ExaminationDetailDto
                {
                    ExamId = medicalRecord.Examination.ExamId,
                    Result = medicalRecord.Examination.Result,
                    Cd4Count = medicalRecord.Examination.Cd4Count,
                    HivLoad = medicalRecord.Examination.HivLoad,
                    ExamDate = medicalRecord.Examination.ExamDate,
                    Status = medicalRecord.Examination.Status,
                    CreatedAt = medicalRecord.Examination.CreatedAt
                };
            }

            // Map thông tin Customized ARV Protocol và các ARV liên quan
            if (medicalRecord.CustomProtocol != null)
            {
                result.CustomizedProtocol = new CustomizedArvProtocolDto
                {
                    CustomProtocolId = medicalRecord.CustomProtocol.CustomProtocolId,
                    Name = medicalRecord.CustomProtocol.Name,
                    Description = medicalRecord.CustomProtocol.Description,
                    Status = medicalRecord.CustomProtocol.Status,
                    BaseProtocolName = medicalRecord.CustomProtocol.BaseProtocol?.Name
                };

                // Map danh sách ARV trong protocol
                // Thông tin ARV được lấy từ CustomizedArvProtocolDetail thông qua quan hệ:
                // MedicalRecord -> CustomizedArvProtocol -> CustomizedArvProtocolDetails -> ARV
                if (medicalRecord.CustomProtocol.Details != null)
                {
                    result.CustomizedProtocol.ArvDetails = medicalRecord.CustomProtocol.Details
                        .Where(d => d.Arv != null)
                        .Select(d => new ArvDetailInProtocolDto
                        {
                            ArvId = d.ArvId,
                            ArvName = d.Arv.Name,
                            ArvDescription = d.Arv.Description,
                            Dosage = d.Dosage,
                            UsageInstruction = d.UsageInstruction,
                            Status = d.Status
                        })
                        .ToList();
                }
            }

            return result;
        }
    }
}