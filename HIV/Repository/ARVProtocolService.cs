using HIV.DTOs.DTOARVs;
using HIV.Interfaces.ARVinterfaces;
using HIV.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                .Where(p => p.Status != "DELETED")
                .Select(p => new ARVProtocolDto
                {
                    ProtocolId = p.ProtocolId,
                    Name = p.Name,
                    Description = p.Description,
                    Status = p.Status
                })
                .ToListAsync();
        }

        public async Task<ServiceResult<ARVProtocolDto>> GetFullProtocolByIdAsync(int id)
        {
            try
            {
                var protocol = await _context.ARVProtocols
                    .Include(p => p.Details)
                    .ThenInclude(d => d.Arv)
                    .FirstOrDefaultAsync(p => p.ProtocolId == id && p.Status != "DELETED");

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
                            DetailId = d.Id,
                            ArvId = d.ArvId,
                            ArvName = d.Arv?.Name,
                            Dosage = d.Dosage,
                            UsageInstruction = d.UsageInstruction,
                            Status = d.Status
                        }).ToList()
                };

                return ServiceResult<ARVProtocolDto>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<ARVProtocolDto>.Failure($"Lỗi khi lấy dữ liệu: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ARVProtocolDto>> CreateWithDetailsAsync(CreateARVProtocolWithDetailsDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return ServiceResult<ARVProtocolDto>.Failure("Tên protocol không được để trống");

                if (await _context.ARVProtocols.AnyAsync(p => p.Name == dto.Name))
                    return ServiceResult<ARVProtocolDto>.Failure("Tên protocol đã tồn tại");

                if (dto.Details == null || !dto.Details.Any())
                    return ServiceResult<ARVProtocolDto>.Failure("Protocol phải có ít nhất một ARV");

                // Validate ARVs exist
                var arvIds = dto.Details.Select(d => d.ArvId).Distinct().ToList();
                var existingArvs = await _context.ARVs
                    .Where(a => arvIds.Contains(a.ArvId))
                    .Select(a => a.ArvId)
                    .ToListAsync();

                var invalidArvIds = arvIds.Except(existingArvs).ToList();
                if (invalidArvIds.Any())
                    return ServiceResult<ARVProtocolDto>.Failure($"Không tìm thấy ARV với ID: {string.Join(", ", invalidArvIds)}");

                // Create protocol first (without details)
                var protocol = new ARVProtocol
                {
                    Name = dto.Name.Trim(),
                    Description = dto.Description?.Trim(),
                    Status = dto.Status
                };

                _context.ARVProtocols.Add(protocol);
                await _context.SaveChangesAsync(); // Save to get ProtocolId

                // Create details separately to avoid complex SQL generation
                var details = new List<ARVProtocolDetail>();
                foreach (var detailDto in dto.Details)
                {
                    var detail = new ARVProtocolDetail
                    {
                        ProtocolId = protocol.ProtocolId,
                        ArvId = detailDto.ArvId,
                        Dosage = detailDto.Dosage?.Trim(),
                        UsageInstruction = detailDto.UsageInstruction?.Trim(),
                        Status = "ACTIVE"
                    };
                    details.Add(detail);
                }

                _context.ARVProtocolDetails.AddRange(details);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Load the complete protocol with details
                var result = await GetFullProtocolByIdAsync(protocol.ProtocolId);
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ServiceResult<ARVProtocolDto>.Failure($"Lỗi khi tạo protocol: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ARVProtocolDto>> UpdateProtocolWithDetailsAsync(int protocolId, UpdateARVProtocolWithDetailsDto dto)
        {
            try
            {
                // Get existing protocol
                var protocol = await _context.ARVProtocols
                    .Include(p => p.Details)
                    .FirstOrDefaultAsync(p => p.ProtocolId == protocolId && p.Status != "DELETED");

                if (protocol == null)
                    return ServiceResult<ARVProtocolDto>.Failure("Không tìm thấy protocol");

                // Update protocol info
                protocol.Name = dto.Name;
                protocol.Description = dto.Description;
                protocol.Status = dto.Status;

                // Process details
                var existingDetailIds = protocol.Details.Select(d => d.Id).ToList();
                var updatedDetailIds = dto.Details.Where(d => d.DetailId > 0).Select(d => d.DetailId).ToList();

                // Remove details not in update list
                var detailsToRemove = protocol.Details
                    .Where(d => !updatedDetailIds.Contains(d.Id))
                    .ToList();

                foreach (var detail in detailsToRemove)
                {
                    detail.Status = "INACTIVE";
                }

                // Update or add details
                foreach (var detailDto in dto.Details)
                {
                    if (detailDto.DetailId > 0) // Update existing
                    {
                        var detail = protocol.Details.FirstOrDefault(d => d.Id == detailDto.DetailId);
                        if (detail != null)
                        {
                            detail.ArvId = detailDto.ArvId;
                            detail.Dosage = detailDto.Dosage;
                            detail.UsageInstruction = detailDto.UsageInstruction;
                            detail.Status = detailDto.Status;
                        }
                    }
                    else // Add new
                    {
                        protocol.Details.Add(new ARVProtocolDetail
                        {
                            ArvId = detailDto.ArvId,
                            Dosage = detailDto.Dosage,
                            UsageInstruction = detailDto.UsageInstruction,
                            Status = detailDto.Status
                        });
                    }
                }

                await _context.SaveChangesAsync();

                // Return updated protocol
                return await GetFullProtocolByIdAsync(protocolId);
            }
            catch (Exception ex)
            {
                return ServiceResult<ARVProtocolDto>.Failure($"Lỗi khi cập nhật protocol: {ex.Message}");
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.ARVProtocols.FindAsync(id);
            if (entity == null) return false;

            entity.Status = "DELETED";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ServiceResult<ARVProDetailDto>> AddSingleARVToProtocolAsync(int protocolId, AddARVToProtocolDto dto)
        {
            try
            {
                // Validate protocol exists
                var protocol = await _context.ARVProtocols.FindAsync(protocolId);
                if (protocol == null)
                    return ServiceResult<ARVProDetailDto>.Failure("Không tìm thấy protocol");

                // Validate ARV exists
                var arv = await _context.ARVs.FindAsync(dto.ArvId);
                if (arv == null)
                    return ServiceResult<ARVProDetailDto>.Failure("Không tìm thấy ARV");

                // Create new detail
                var newDetail = new ARVProtocolDetail
                {
                    ProtocolId = protocolId,
                    ArvId = dto.ArvId,
                    Dosage = dto.Dosage,
                    UsageInstruction = dto.UsageInstruction,
                    Status = dto.Status
                };

                _context.ARVProtocolDetails.Add(newDetail);
                await _context.SaveChangesAsync();

                // Return result
                var result = new ARVProDetailDto
                {
                    DetailId = newDetail.Id,
                    ArvId = newDetail.ArvId,
                    ArvName = arv.Name,
                    Dosage = newDetail.Dosage,
                    UsageInstruction = newDetail.UsageInstruction,
                    Status = newDetail.Status
                };

                return ServiceResult<ARVProDetailDto>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<ARVProDetailDto>.Failure($"Lỗi khi thêm ARV: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ARVProDetailDto>> UpdateProtocolDetailAsync(int protocolId, int detailId, UpdateARVProtocolDetailDto dto)
        {
            try
            {
                // Validate IDs
                if (protocolId <= 0 || detailId <= 0 || dto.DetailId != detailId)
                    return ServiceResult<ARVProDetailDto>.Failure("ID không hợp lệ");

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
                detail.Dosage = dto.Dosage;
                detail.UsageInstruction = dto.UsageInstruction;
                detail.Status = dto.Status;

                await _context.SaveChangesAsync();

                // Return updated detail
                var result = new ARVProDetailDto
                {
                    DetailId = detail.Id,
                    ArvId = detail.ArvId,
                    ArvName = detail.Arv?.Name,
                    Dosage = detail.Dosage,
                    UsageInstruction = detail.UsageInstruction,
                    Status = detail.Status
                };

                return ServiceResult<ARVProDetailDto>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<ARVProDetailDto>.Failure($"Lỗi khi cập nhật: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> HardRemoveDetailFromProtocolAsync(int protocolId, int detailId)
        {
            try
            {
                var detail = await _context.ARVProtocolDetails
                    .FirstOrDefaultAsync(d => d.Id == detailId && d.ProtocolId == protocolId);

                if (detail == null)
                    return ServiceResult<bool>.Failure("Không tìm thấy chi tiết protocol");

                _context.ARVProtocolDetails.Remove(detail);
                await _context.SaveChangesAsync();

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Lỗi khi xóa: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<ARVProDetailDto>>> GetProtocolActiveDetailsAsync(int protocolId)
        {
            try
            {
                var details = await _context.ARVProtocolDetails
                    .Where(d => d.ProtocolId == protocolId && d.Status == "ACTIVE")
                    .Include(d => d.Arv)
                    .Select(d => new ARVProDetailDto
                    {
                        DetailId = d.Id,
                        ArvId = d.ArvId,
                        ArvName = d.Arv.Name,
                        Dosage = d.Dosage,
                        UsageInstruction = d.UsageInstruction,
                        Status = d.Status
                    })
                    .ToListAsync();

                return ServiceResult<List<ARVProDetailDto>>.Success(details);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<ARVProDetailDto>>.Failure($"Lỗi khi lấy danh sách: {ex.Message}");
            }
        }
    }
}