using HIV.DTOs.DTOARVs;
using HIV.Interfaces.ARVinterfaces;
using HIV.Models;
using Microsoft.EntityFrameworkCore;

namespace HIV.Repository
{
    public class CustomizedArvProtocolDetailService : ICustomizedArvProtocolDetailService
    {
        private readonly AppDbContext _context;

        public CustomizedArvProtocolDetailService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomizedArvProtocolDetailDto>> GetAllAsync()
        {
            return await _context.CustomizedARVProtocolDetails
                .Select(x => new CustomizedArvProtocolDetailDto
                {
                    Id = x.Id,
                    CustomProtocolId = x.CustomProtocolId,
                    ArvId = x.ArvId,
                    Dosage = x.Dosage,
                    UsageInstruction = x.UsageInstruction,
                    Status = x.Status
                })
                .ToListAsync();
        }

        public async Task<CustomizedArvProtocolDetailDto?> GetByIdAsync(int id)
        {
            var x = await _context.CustomizedARVProtocolDetails.FindAsync(id);
            if (x == null) return null;

            return new CustomizedArvProtocolDetailDto
            {
                Id = x.Id,
                CustomProtocolId = x.CustomProtocolId,
                ArvId = x.ArvId,
                Dosage = x.Dosage,
                UsageInstruction = x.UsageInstruction,
                Status = x.Status
            };
        }

        public async Task<CustomizedArvProtocolDetailDto> CreateAsync(CreateCustomizedArvProtocolDetailDto dto)
        {
            var entity = new CustomizedArvProtocolDetail
            {
                CustomProtocolId = dto.CustomProtocolId,
                ArvId = dto.ArvId,
                Dosage = dto.Dosage,
                UsageInstruction = dto.UsageInstruction,
                Status = dto.Status
            };

            _context.CustomizedARVProtocolDetails.Add(entity);
            await _context.SaveChangesAsync();

            return new CustomizedArvProtocolDetailDto
            {
                Id = entity.Id,
                CustomProtocolId = entity.CustomProtocolId,
                ArvId = entity.ArvId,
                Dosage = entity.Dosage,
                UsageInstruction = entity.UsageInstruction,
                Status = entity.Status
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateCustomizedArvProtocolDetailDto dto)
        {
            var entity = await _context.CustomizedARVProtocolDetails.FindAsync(id);
            if (entity == null) return false;

            entity.Dosage = dto.Dosage;
            entity.UsageInstruction = dto.UsageInstruction;
            entity.Status = dto.Status;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.CustomizedARVProtocolDetails.FindAsync(id);
            if (entity == null) return false;

            entity.Status = "DELETED";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
