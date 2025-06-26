using HIV.DTOs.DTOARVs;
using HIV.Interfaces.ARVinterfaces;
using HIV.Models;
using Microsoft.EntityFrameworkCore;

namespace HIV.Repository
{
    public class ARVProtocolService : IARVProtocolService
    {
        private readonly AppDbContext _context;

        public ARVProtocolService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ARVProtocolDto>> GetAllAsync()
        {
            return await _context.ARVProtocols
                .Select(x => new ARVProtocolDto
                {
                    ProtocolId = x.ProtocolId,
                    Name = x.Name,
                    Description = x.Description,
                    Status = x.Status
                })
                .ToListAsync();
        }

        public async Task<ARVProtocolDto?> GetByIdAsync(int id)
        {
            var x = await _context.ARVProtocols.FindAsync(id);
            if (x == null) return null;

            return new ARVProtocolDto
            {
                ProtocolId = x.ProtocolId,
                Name = x.Name,
                Description = x.Description,
                Status = x.Status
            };
        }

        public async Task<IEnumerable<ARVProDetailDto>> GetARVDetailsByProtocolIdAsync(int protocolId)
        {
            return await _context.ARVProtocolDetails
                .Where(d => d.ProtocolId == protocolId && d.Status == "ACTIVE")
                .Include(d => d.Arv)
                .Select(d => new ARVProDetailDto
                {
                    ArvId = d.ArvId,
                    ArvName = d.Arv.Name,
                    Dosage = d.Dosage,
                    UsageInstruction = d.UsageInstruction
                })
                .ToListAsync();
        }

        public async Task<ARVProtocolDto> CreateAsync(CreateARVProtocolDto dto)
        {
            var entity = new ARVProtocol
            {
                Name = dto.Name,
                Description = dto.Description,
                Status = dto.Status
            };

            _context.ARVProtocols.Add(entity);
            await _context.SaveChangesAsync();

            return new ARVProtocolDto
            {
                ProtocolId = entity.ProtocolId,
                Name = entity.Name,
                Description = entity.Description,
                Status = entity.Status
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateARVProtocolDto dto)
        {
            var entity = await _context.ARVProtocols.FindAsync(id);
            if (entity == null) return false;

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Status = dto.Status;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.ARVProtocols.FindAsync(id);
            if (entity == null) return false;

            entity.Status = "DELETED";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ServiceResult<ARVProtocolDto>> CreateWithDetailsAsync(CreateARVProtocolWithDetailsDto dto)
        {
            try
            {
                // Validate protocol name
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return ServiceResult<ARVProtocolDto>.Failure("Tên protocol không được để trống");

                // Check duplicate protocol name
                if (await _context.ARVProtocols.AnyAsync(p => p.Name == dto.Name))
                    return ServiceResult<ARVProtocolDto>.Failure("Tên protocol đã tồn tại");

                // Validate details
                if (dto.Details == null || !dto.Details.Any())
                    return ServiceResult<ARVProtocolDto>.Failure("Protocol phải có ít nhất một ARV");

                // Check ARV exists
                var arvIds = dto.Details.Select(d => d.ArvId).Distinct().ToList();
                var existingArvs = await _context.ARVs
                    .Where(a => arvIds.Contains(a.ArvId))
                    .Select(a => a.ArvId)
                    .ToListAsync();

                var invalidArvIds = arvIds.Except(existingArvs).ToList();
                if (invalidArvIds.Any())
                    return ServiceResult<ARVProtocolDto>.Failure($"Không tìm thấy ARV với ID: {string.Join(", ", invalidArvIds)}");

                // Create new protocol with details
                var protocol = new ARVProtocol
                {
                    Name = dto.Name.Trim(),
                    Description = dto.Description?.Trim(),
                    Status = dto.Status,
                    Details = dto.Details.Select(d => new ARVProtocolDetail
                    {
                        ArvId = d.ArvId,
                        Dosage = d.Dosage.Trim(),
                        UsageInstruction = d.UsageInstruction?.Trim(),
                        Status = d.Status
                    }).ToList()
                };

                _context.ARVProtocols.Add(protocol);
                await _context.SaveChangesAsync();

                // Return result with details
                var result = new ARVProtocolDto
                {
                    ProtocolId = protocol.ProtocolId,
                    Name = protocol.Name,
                    Description = protocol.Description,
                    Status = protocol.Status,
                    Details = protocol.Details.Select(d => new ARVProDetailDto
                    {
                        ArvId = d.ArvId,
                        ArvName = d.Arv?.Name,
                        Dosage = d.Dosage,
                        UsageInstruction = d.UsageInstruction
                    }).ToList()
                };

                return ServiceResult<ARVProtocolDto>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<ARVProtocolDto>.Failure($"Lỗi khi tạo protocol: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ARVProtocolDto>> AddDetailsToProtocolAsync(int protocolId, List<CreateARVProtocolDetailDto> details)
        {
            try
            {
                // Get protocol with details
                var protocol = await _context.ARVProtocols
                    .Include(p => p.Details)
                    .FirstOrDefaultAsync(p => p.ProtocolId == protocolId);

                if (protocol == null)
                    return ServiceResult<ARVProtocolDto>.Failure("Không tìm thấy protocol");

                // Validate details
                if (details == null || !details.Any())
                    return ServiceResult<ARVProtocolDto>.Failure("Danh sách ARV không được trống");

                // Check ARV exists
                var arvIds = details.Select(d => d.ArvId).Distinct().ToList();
                var existingArvs = await _context.ARVs
                    .Where(a => arvIds.Contains(a.ArvId))
                    .Select(a => a.ArvId)
                    .ToListAsync();

                var invalidArvIds = arvIds.Except(existingArvs).ToList();
                if (invalidArvIds.Any())
                    return ServiceResult<ARVProtocolDto>.Failure($"Không tìm thấy ARV với ID: {string.Join(", ", invalidArvIds)}");

                // Add new details
                foreach (var detailDto in details)
                {
                    protocol.Details.Add(new ARVProtocolDetail
                    {
                        ArvId = detailDto.ArvId,
                        Dosage = detailDto.Dosage.Trim(),
                        UsageInstruction = detailDto.UsageInstruction?.Trim(),
                        Status = detailDto.Status
                    });
                }

                await _context.SaveChangesAsync();

                // Return updated protocol
                var result = new ARVProtocolDto
                {
                    ProtocolId = protocol.ProtocolId,
                    Name = protocol.Name,
                    Description = protocol.Description,
                    Status = protocol.Status,
                    Details = protocol.Details.Select(d => new ARVProDetailDto
                    {
                        ArvId = d.ArvId,
                        ArvName = d.Arv?.Name,
                        Dosage = d.Dosage,
                        UsageInstruction = d.UsageInstruction
                    }).ToList()
                };

                return ServiceResult<ARVProtocolDto>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<ARVProtocolDto>.Failure($"Lỗi khi thêm ARV: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ARVProDetailDto>> UpdateProtocolDetailAsync(int protocolId, int detailId, CreateARVProtocolDetailDto dto)
        {
            try
            {
                // Get protocol detail
                var detail = await _context.ARVProtocolDetails
                    .Include(d => d.Arv)
                    .FirstOrDefaultAsync(d => d.Id == detailId && d.ProtocolId == protocolId);

                if (detail == null)
                    return ServiceResult<ARVProDetailDto>.Failure("Không tìm thấy chi tiết protocol");

                // Check ARV exists
                var arvExists = await _context.ARVs.AnyAsync(a => a.ArvId == dto.ArvId);
                if (!arvExists)
                    return ServiceResult<ARVProDetailDto>.Failure("Không tìm thấy ARV");

                // Update detail
                detail.ArvId = dto.ArvId;
                detail.Dosage = dto.Dosage.Trim();
                detail.UsageInstruction = dto.UsageInstruction?.Trim();
                detail.Status = dto.Status;

                await _context.SaveChangesAsync();

                // Return updated detail
                var result = new ARVProDetailDto
                {
                    ArvId = detail.ArvId,
                    ArvName = detail.Arv?.Name,
                    Dosage = detail.Dosage,
                    UsageInstruction = detail.UsageInstruction,
                    DetailId = detail.Id
                };

                return ServiceResult<ARVProDetailDto>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<ARVProDetailDto>.Failure($"Lỗi khi cập nhật: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> RemoveDetailFromProtocolAsync(int protocolId, int detailId)
        {
            try
            {
                var detail = await _context.ARVProtocolDetails
                    .FirstOrDefaultAsync(d => d.Id == detailId && d.ProtocolId == protocolId);

                if (detail == null)
                    return ServiceResult<bool>.Failure("Không tìm thấy chi tiết protocol");

                detail.Status = "INACTIVE";
                await _context.SaveChangesAsync();

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Lỗi khi xóa: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ARVProtocolDto>> GetFullProtocolByIdAsync(int id)
        {
            try
            {
                var protocol = await _context.ARVProtocols
                    .Include(p => p.Details)
                        .ThenInclude(d => d.Arv)
                    .FirstOrDefaultAsync(p => p.ProtocolId == id);

                if (protocol == null)
                    return ServiceResult<ARVProtocolDto>.Failure("Không tìm thấy protocol");

                var result = new ARVProtocolDto
                {
                    ProtocolId = protocol.ProtocolId,
                    Name = protocol.Name,
                    Description = protocol.Description,
                    Status = protocol.Status,
                    Details = protocol.Details
                        .Where(d => d.Status == "ACTIVE")
                        .Select(d => new ARVProDetailDto
                        {
                            ArvId = d.ArvId,
                            ArvName = d.Arv?.Name,
                            Dosage = d.Dosage,
                            UsageInstruction = d.UsageInstruction,
                            DetailId = d.Id
                        }).ToList()
                };

                return ServiceResult<ARVProtocolDto>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<ARVProtocolDto>.Failure($"Lỗi khi lấy dữ liệu: {ex.Message}");
            }
        }
    }

}
