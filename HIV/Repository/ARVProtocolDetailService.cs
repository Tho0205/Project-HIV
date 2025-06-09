using HIV.DTOs.DTOARVs;
using HIV.Interfaces.ARVinterfaces;
using HIV.Models;
using Microsoft.EntityFrameworkCore;

namespace HIV.Repository
{
    public class ARVProtocolDetailService : IARVProtocolDetailService
    {
        private readonly AppDbContext _context;

        public ARVProtocolDetailService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ARVProtocolDetailDto>> GetAllAsync()
        {
            return await _context.ARVProtocolDetails
                .Select(x => new ARVProtocolDetailDto
                {
                    Id = x.Id,
                    ProtocolId = x.ProtocolId,
                    ArvId = x.ArvId,
                    Dosage = x.Dosage,
                    UsageInstruction = x.UsageInstruction,
                    Status = x.Status
                })
                .ToListAsync();
        }

        public async Task<ARVProtocolDetailDto?> GetByIdAsync(int id)
        {
            var x = await _context.ARVProtocolDetails.FindAsync(id);
            if (x == null) return null;

            return new ARVProtocolDetailDto
            {
                Id = x.Id,
                ProtocolId = x.ProtocolId,
                ArvId = x.ArvId,
                Dosage = x.Dosage,
                UsageInstruction = x.UsageInstruction,
                Status = x.Status
            };
        }

        public async Task<ARVProtocolDetailDto> CreateAsync(CreateARVProtocolDetailDto dto)
        {
            var entity = new ARVProtocolDetail
            {
                ProtocolId = dto.ProtocolId,
                ArvId = dto.ArvId,
                Dosage = dto.Dosage,
                UsageInstruction = dto.UsageInstruction,
                Status = dto.Status
            };

            _context.ARVProtocolDetails.Add(entity);
            await _context.SaveChangesAsync();

            return new ARVProtocolDetailDto
            {
                Id = entity.Id,
                ProtocolId = entity.ProtocolId,
                ArvId = entity.ArvId,
                Dosage = entity.Dosage,
                UsageInstruction = entity.UsageInstruction,
                Status = entity.Status
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateARVProtocolDetailDto dto)
        {
            var entity = await _context.ARVProtocolDetails.FindAsync(id);
            if (entity == null) return false;

            entity.Dosage = dto.Dosage;
            entity.UsageInstruction = dto.UsageInstruction;
            entity.Status = dto.Status;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.ARVProtocolDetails.FindAsync(id);
            if (entity == null) return false;

            entity.Status = "DELETED";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
