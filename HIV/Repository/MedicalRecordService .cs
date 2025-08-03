using HIV.DTOs;
using HIV.DTOs.DTOAppointment;
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

        public async Task<MedicalRecordDto> CreateByAppointmentIdAsync(CreateMedicalRecordByAppointmentDto dto)
        {
            // 1. Lấy thông tin Appointment
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.AppointmentId == dto.AppointmentId);

            if (appointment == null)
            {
                throw new ArgumentException($"Appointment with ID {dto.AppointmentId} not found");
            }

            // 2. TÌM EXAMINATION - UU TIÊN AppointmentId CHÍNH XÁC TRƯỚC
            Examination? examination = null;

            // Bước 2a: Tìm theo AppointmentId chính xác trước
            examination = await _context.Examinations
                .FirstOrDefaultAsync(e => e.AppointmentId == dto.AppointmentId);

            // Bước 2b: Nếu không tìm thấy, mới tìm theo PatientId + DoctorId + Date
            if (examination == null)
            {
                examination = await _context.Examinations
                    .Where(e => e.PatientId == appointment.PatientId
                        && e.DoctorId == appointment.DoctorId
                        && e.ExamDate == DateOnly.FromDateTime(appointment.AppointmentDate))
                    .FirstOrDefaultAsync();
            }

            if (examination == null)
            {
                throw new ArgumentException($"No examination found for appointment {dto.AppointmentId}");
            }

            // 3. KIỂM TRA EXAMID ĐÃ CÓ MEDICALRECORD CHƯA
            var existingRecord = await _context.MedicalRecords
                .FirstOrDefaultAsync(mr => mr.ExamId == examination.ExamId);

            if (existingRecord != null)
            {
                throw new InvalidOperationException(
                    $"Examination {examination.ExamId} already has MedicalRecord {existingRecord.RecordId}. " +
                    $"Cannot create duplicate medical record."
                );
            }

            // 4. Tìm CustomProtocol - UU TIÊN AppointmentId CHÍNH XÁC
            CustomizedArvProtocol? customProtocol = null;

            // Bước 4a: Tìm theo AppointmentId chính xác trước
            customProtocol = await _context.CustomizedARVProtocols
                .FirstOrDefaultAsync(cp => cp.AppointmentId == dto.AppointmentId && cp.Status == "ACTIVE");

            // Bước 4b: Nếu không tìm thấy, tìm theo PatientId + DoctorId
            if (customProtocol == null)
            {
                customProtocol = await _context.CustomizedARVProtocols
                    .Where(cp => cp.PatientId == appointment.PatientId
                        && cp.DoctorId == appointment.DoctorId
                        && cp.Status == "ACTIVE")
                    .OrderByDescending(cp => cp.CustomProtocolId) // Lấy mới nhất
                    .FirstOrDefaultAsync();
            }

            // 5. Tạo MedicalRecord
            var entity = new MedicalRecord
            {
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                ExamId = examination.ExamId,
                CustomProtocolId = customProtocol?.CustomProtocolId,
                AppointmentId = dto.AppointmentId,
                ExamDate = dto.ExamDate ?? appointment.AppointmentDate,
                ExamTime = dto.ExamTime ?? TimeSpan.FromHours(9),
                Summary = dto.Summary ?? $"Medical record created from appointment #{dto.AppointmentId}",
                IssuedAt = DateTime.UtcNow,
                Status = "ACTIVE"
            };

            _context.MedicalRecords.Add(entity);
            await _context.SaveChangesAsync();

            // 6. Return DTO
            return new MedicalRecordDto
            {
                RecordId = entity.RecordId,
                PatientId = entity.PatientId,
                DoctorId = entity.DoctorId,
                ExamId = entity.ExamId,
                CustomProtocolId = entity.CustomProtocolId,
                AppointmentId = entity.AppointmentId,
                ExamDate = entity.ExamDate,
                ExamTime = entity.ExamTime,
                Summary = entity.Summary ?? "",
                Status = entity.Status ?? "",
                IssuedAt = entity.IssuedAt,
                DoctorName = appointment.Doctor?.FullName ?? "",
                PatientName = appointment.Patient?.FullName ?? ""
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
                .Include(m => m.Examination)
                .Include(m => m.CustomProtocol)
                    .ThenInclude(cp => cp.BaseProtocol)
                .Include(m => m.CustomProtocol)
                    .ThenInclude(cp => cp.Details)
                        .ThenInclude(d => d.Arv)
                .Include(m => m.Appointment)
                .Where(m => m.PatientId == patientId && m.Status != "DELETED")
                .Select(m => new MedicalRecordDto
                {
                    RecordId = m.RecordId,
                    PatientId = m.PatientId,
                    DoctorId = m.DoctorId,
                    ExamId = m.ExamId,
                    CustomProtocolId = m.CustomProtocolId,
                    AppointmentId = m.AppointmentId,
                    ExamDate = m.ExamDate,
                    ExamTime = m.ExamTime,
                    Summary = m.Summary != null ? m.Summary : "",
                    Status = m.Status,
                    IssuedAt = m.IssuedAt,
                    DoctorName = m.Doctor != null ? m.Doctor.FullName : "Unknown Doctor",
                    PatientName = m.Patient != null ? m.Patient.FullName : "Unknown Patient",

                    // CHỈ LẤY EXAMINATION THUỘC CÙNG APPOINTMENT
                    ExaminationInfo = m.Examination != null && m.Examination.AppointmentId == m.AppointmentId
                        ? new ExaminationDto
                        {
                            ExamId = m.Examination.ExamId,
                            Result = m.Examination.Result != null ? m.Examination.Result : "No result",
                            Cd4Count = m.Examination.Cd4Count,
                            HivLoad = m.Examination.HivLoad,
                            ExamDate = m.Examination.ExamDate,
                            Status = m.Examination.Status != null ? m.Examination.Status : "UNKNOWN",
                            CreatedAt = m.Examination.CreatedAt
                        } : null,

                    // CHỈ LẤY CUSTOM PROTOCOL THUỘC CÙNG APPOINTMENT
                    CustomProtocolInfo = m.CustomProtocol != null && m.CustomProtocol.AppointmentId == m.AppointmentId
                        ? new CustomizedArvProtocolDto
                        {
                            CustomProtocolId = m.CustomProtocol.CustomProtocolId,
                            Name = m.CustomProtocol.Name != null ? m.CustomProtocol.Name : $"Protocol #{m.CustomProtocol.CustomProtocolId}",
                            Description = m.CustomProtocol.Description != null ? m.CustomProtocol.Description : "No description available",
                            Status = m.CustomProtocol.Status != null ? m.CustomProtocol.Status : "UNKNOWN",
                            BaseProtocolName = m.CustomProtocol.BaseProtocol != null ? m.CustomProtocol.BaseProtocol.Name : "No base protocol",

                            // ARV Details - CHỈ LẤY TỪ PROTOCOL CỦA APPOINTMENT NÀY
                            ArvDetails = m.CustomProtocol.Details != null && m.CustomProtocol.Details.Any() ?
                                m.CustomProtocol.Details
                                    .Where(d => d.Arv != null && d.Status == "ACTIVE")
                                    .Select(d => new ArvDetailInProtocolDto
                                    {
                                        ArvId = d.ArvId,
                                        ArvName = d.Arv.Name != null ? d.Arv.Name : "Unknown ARV",
                                        ArvDescription = d.Arv.Description != null ? d.Arv.Description : "No description",
                                        Dosage = d.Dosage != null ? d.Dosage : "Not specified",
                                        UsageInstruction = d.UsageInstruction != null ? d.UsageInstruction : "Follow doctor's instruction",
                                        Status = d.Status != null ? d.Status : "ACTIVE"
                                    }).ToList()
                                : new List<ArvDetailInProtocolDto>()
                        } : null,

                    // Appointment Info
                    AppointmentInfo = m.Appointment != null ? new AppointmentDto
                    {
                        AppointmentId = m.Appointment.AppointmentId,
                        AppointmentDate = m.Appointment.AppointmentDate,
                        Status = m.Appointment.Status != null ? m.Appointment.Status : "UNKNOWN",
                        AppointmentType = m.Appointment.AppoinmentType != null ? m.Appointment.AppoinmentType : "Regular",
                        Note = m.Appointment.Note != null ? m.Appointment.Note : "",
                        IsAnonymous = m.Appointment.IsAnonymous
                    } : null
                })
                .OrderByDescending(m => m.ExamDate != null ? m.ExamDate : DateTime.MinValue)
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