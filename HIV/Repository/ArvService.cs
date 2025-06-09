using HIV.DTOs.DTOARVs;
using HIV.Interfaces.ARVinterfaces;
using HIV.Models;
using Microsoft.EntityFrameworkCore;

namespace HIV.Repository
{
    public class ArvService : IArvService
    {
        private readonly AppDbContext _context;

        public ArvService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ArvDto>> GetAllAsync()
        {
            return await _context.ARVs
                .Select(x => new ArvDto
                {
                    ArvId = x.ArvId,
                    Name = x.Name,
                    Description = x.Description,
                    Status = x.Status
                })
                .ToListAsync();
        }

        public async Task<ArvDto?> GetByIdAsync(int id)
        {
            var x = await _context.ARVs.FindAsync(id);
            if (x == null) return null;

            return new ArvDto
            {
                ArvId = x.ArvId,
                Name = x.Name,
                Description = x.Description,
                Status = x.Status
            };
        }

        public async Task<ArvDto> CreateAsync(CreateArvDto dto)
        {
            var entity = new Arv
            {
                Name = dto.Name,
                Description = dto.Description,
                Status = dto.Status
            };

            _context.ARVs.Add(entity);
            await _context.SaveChangesAsync();

            return new ArvDto
            {
                ArvId = entity.ArvId,
                Name = entity.Name,
                Description = entity.Description,
                Status = entity.Status
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateArvDto dto)
        {
            var entity = await _context.ARVs.FindAsync(id);
            if (entity == null) return false;

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Status = dto.Status;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.ARVs.FindAsync(id);
            if (entity == null) return false;

            entity.Status = "DELETED";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
