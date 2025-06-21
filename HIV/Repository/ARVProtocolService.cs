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
    }
}
