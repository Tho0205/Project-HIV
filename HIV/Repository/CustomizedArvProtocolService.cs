using HIV.DTOs;
using HIV.DTOs.DTOARVs;
using HIV.Interfaces;
using HIV.Interfaces.ARVinterfaces;
using HIV.Models;
using Microsoft.EntityFrameworkCore;

namespace HIV.Repository
{
    public class CustomizedArvProtocolService : ICustomizedArvProtocolService
    {
        private readonly AppDbContext _context;
        private readonly IMedicalRecordService _medicalRecordService;

        public CustomizedArvProtocolService(AppDbContext context, IMedicalRecordService medicalRecordService)
        {
            _context = context;
            _medicalRecordService = medicalRecordService;
        }

        public async Task<List<PatientWithProtocolDto>> GetPatientsWithProtocolsAsync(int doctorId)
        {
            // Bước 1: Lấy danh sách phác đồ mới nhất mỗi bệnh nhân theo DoctorId
            var allActiveProtocols = await _context.CustomizedARVProtocols
                .Where(cp => cp.DoctorId == doctorId && cp.Status == "ACTIVE")
                .Include(cp => cp.Patient)
                    .ThenInclude(p => p.ExaminationsAsPatient)
                .ToListAsync();

            // Bước 2: Nhóm theo bệnh nhân và lấy phác đồ mới nhất (xử lý trong bộ nhớ)
            var latestProtocolsPerPatient = allActiveProtocols
                .GroupBy(cp => cp.PatientId)
                .Select(g => g.OrderByDescending(cp => cp.CustomProtocolId).First())
                .ToList();

            // Bước 3: Dựng DTO
            var result = latestProtocolsPerPatient.Select(cp => new PatientWithProtocolDto
            {
                PatientId = cp.PatientId ?? 0,
                FullName = cp.Patient?.FullName ?? "Unknown",
                Phone = cp.Patient?.Phone ?? "Unknown",
                LatestExamination = cp.Patient?.ExaminationsAsPatient?
                    .Where(e => e.Status == "ACTIVE" && e.ExamDate <= DateOnly.FromDateTime(DateTime.Now))
                    .OrderByDescending(e => e.ExamDate)
                    .Select(e => new LatestExaminationDto
                    {
                        ExamDate = e.ExamDate,
                        Cd4Count = e.Cd4Count,
                        HivLoad = e.HivLoad,
                        Result = e.Result ?? "No result"
                    })
                    .FirstOrDefault(),
                CurrentProtocol = new ProtocolInfoDto
                {
                    ProtocolId = cp.CustomProtocolId,
                    Name = cp.Name ?? "Unnamed Protocol",
                    IsCustom = true
                }
            }).ToList();

            return result;
        }


        public async Task<FullCustomProtocolDto?> GetPatientCurrentProtocolAsync(int patientId)
        {
            var protocol = await _context.CustomizedARVProtocols
                .Include(cp => cp.Details)
                .ThenInclude(d => d.Arv)
                .Include(cp => cp.BaseProtocol)
                .Where(cp => cp.PatientId == patientId && cp.Status == "ACTIVE")
                .OrderByDescending(cp => cp.CustomProtocolId)
                .FirstOrDefaultAsync();

            if (protocol == null) return null;

            return new FullCustomProtocolDto
            {
                CustomProtocolId = protocol.CustomProtocolId,
                BaseProtocolId = protocol.BaseProtocolId,
                BaseProtocolName = protocol.BaseProtocol?.Name,
                AppointmentId = protocol.AppointmentId,
                Name = protocol.Name ?? "Unnamed Protocol",
                Description = protocol.Description,
                Status = protocol.Status ?? "ACTIVE",
                Details = protocol.Details
                    .Where(d => d.Status == "ACTIVE")
                    .Select(d => new CustomProtocolDetailDto
                    {
                        DetailId = d.Id,
                        ArvId = d.ArvId,
                        ArvName = d.Arv?.Name ?? "Unknown ARV",
                        Dosage = d.Dosage ?? "No dosage",
                        UsageInstruction = d.UsageInstruction,
                        Status = d.Status ?? "ACTIVE"
                    })
                    .ToList()
            };
        }

        public async Task<FullCustomProtocolDto> CreateCustomProtocolAsync(int doctorId, int patientId, CreateCustomProtocolRequest request)
        {
            // Validate ARVs exist
            var arvIds = request.Details.Select(d => d.ArvId).Distinct();
            var existingArvs = await _context.ARVs
                .Where(a => arvIds.Contains(a.ArvId))
                .Select(a => a.ArvId)
                .ToListAsync();

            if (existingArvs.Count != arvIds.Count())
            {
                throw new ArgumentException("One or more ARV IDs are invalid");
            }

            var protocol = new CustomizedArvProtocol
            {
                DoctorId = doctorId,
                PatientId = patientId,
                BaseProtocolId = request.BaseProtocolId,
                AppointmentId = request.AppointmentId,
                Name = request.Name,
                Description = request.Description,
                Status = "ACTIVE",
                Details = request.Details.Select(d => new CustomizedArvProtocolDetail
                {
                    ArvId = d.ArvId,
                    Dosage = d.Dosage,
                    UsageInstruction = d.UsageInstruction,
                    Status = "ACTIVE"
                }).ToList()
            };

            _context.CustomizedARVProtocols.Add(protocol);
            await _context.SaveChangesAsync();

            return (await GetPatientCurrentProtocolAsync(patientId))!;
        }

        public async Task<bool> UpdatePatientProtocolAsync(int patientId, UpdatePatientProtocolRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Đặt tất cả phác đồ hiện tại về COMPLETED, trừ cái được chọn (nếu là custom)
                var currentProtocols = await _context.CustomizedARVProtocols
                    .Where(cp => cp.PatientId == patientId && cp.Status == "ACTIVE" && cp.CustomProtocolId != request.ProtocolId)
                    .ToListAsync();

                foreach (var protocol in currentProtocols)
                {
                    protocol.Status = "COMPLETED";
                }

                _context.CustomizedARVProtocols.UpdateRange(currentProtocols);

                if (request.IsCustom)
                {
                    // Kích hoạt lại phác đồ đã tạo từ trước
                    var protocolToActivate = await _context.CustomizedARVProtocols
                        .FirstOrDefaultAsync(cp => cp.CustomProtocolId == request.ProtocolId);

                    if (protocolToActivate == null) return false;

                    protocolToActivate.Status = "ACTIVE";
                }
                else
                {
                    // Tạo phác đồ mới từ phác đồ chuẩn
                    var standardProtocol = await _context.ARVProtocols
                        .Include(p => p.Details)
                        .FirstOrDefaultAsync(p => p.ProtocolId == request.ProtocolId);

                    if (standardProtocol == null) return false;

                    var lastProtocol = await _context.CustomizedARVProtocols
                        .Where(cp => cp.PatientId == patientId)
                        .OrderByDescending(cp => cp.CustomProtocolId)
                        .FirstOrDefaultAsync();

                    if (lastProtocol?.DoctorId == null) return false;

                    var newCustomProtocol = new CustomizedArvProtocol
                    {
                        DoctorId = lastProtocol.DoctorId.Value,
                        PatientId = patientId,
                        BaseProtocolId = standardProtocol.ProtocolId,
                        AppointmentId = request.AppointmentId,
                        Name = $"Customized {standardProtocol.Name}",
                        Description = standardProtocol.Description,
                        Status = "ACTIVE",
                        Details = standardProtocol.Details
                            .Where(d => d.Status == "ACTIVE")
                            .Select(d => new CustomizedArvProtocolDetail
                            {
                                ArvId = d.ArvId,
                                Dosage = d.Dosage ?? "Default dosage",
                                UsageInstruction = d.UsageInstruction,
                                Status = "ACTIVE"
                            })
                            .ToList()
                    };

                    _context.CustomizedARVProtocols.Add(newCustomProtocol);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<FullCustomProtocolDto>> GetPatientProtocolHistoryAsync(int patientId)
        {
            return await _context.CustomizedARVProtocols
                .Include(cp => cp.Details)
                .ThenInclude(d => d.Arv)
                .Include(cp => cp.BaseProtocol)
                .Where(cp => cp.PatientId == patientId)
                .OrderByDescending(cp => cp.CustomProtocolId)
                .Select(cp => new FullCustomProtocolDto
                {
                    CustomProtocolId = cp.CustomProtocolId,
                    BaseProtocolId = cp.BaseProtocolId,
                    BaseProtocolName = cp.BaseProtocol!.Name,
                    AppointmentId = cp.AppointmentId,
                    Name = cp.Name ?? "Unnamed Protocol",
                    Description = cp.Description,
                    Status = cp.Status ?? "ACTIVE",
                    Details = cp.Details
                        .Where(d => d.Status == "ACTIVE")
                        .Select(d => new CustomProtocolDetailDto
                        {
                            DetailId = d.Id,
                            ArvId = d.ArvId,
                            ArvName = d.Arv!.Name,
                            Dosage = d.Dosage ?? "No dosage",
                            UsageInstruction = d.UsageInstruction,
                            Status = d.Status ?? "ACTIVE"
                        })
                        .ToList()
                })
                .ToListAsync();
        }
        public async Task<bool> UpdateProtocolAsync(int protocolId, UpdateCustomProtocolDto dto)
        {
            var protocol = await _context.CustomizedARVProtocols.FindAsync(protocolId);
            if (protocol == null) return false;

            protocol.Name = dto.Name;
            protocol.Description = dto.Description;

            await _context.SaveChangesAsync();

            return true;
        }

    }
}