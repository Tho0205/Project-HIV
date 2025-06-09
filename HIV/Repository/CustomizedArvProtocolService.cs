using HIV.DTOs.DTOARVs;
using HIV.Interfaces.ARVinterfaces;
using HIV.Models;
using Microsoft.EntityFrameworkCore;

namespace HIV.Repository
{
    public class CustomizedArvProtocolService : ICustomizedArvProtocolService
    {
        private readonly AppDbContext _context;

        public CustomizedArvProtocolService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomizedArvProtocolDto>> GetAllAsync()
        {
            return await _context.CustomizedARVProtocols
                .Select(x => new CustomizedArvProtocolDto
                {
                    CustomProtocolId = x.CustomProtocolId,
                    DoctorId = x.DoctorId,
                    PatientId = x.PatientId,
                    BaseProtocolId = x.BaseProtocolId,
                    Name = x.Name,
                    Description = x.Description,
                    Status = x.Status
                })
                .ToListAsync();
        }

        public async Task<CustomizedArvProtocolDto?> GetByIdAsync(int id)
        {
            var x = await _context.CustomizedARVProtocols.FindAsync(id);
            if (x == null) return null;

            return new CustomizedArvProtocolDto
            {
                CustomProtocolId = x.CustomProtocolId,
                DoctorId = x.DoctorId,
                PatientId = x.PatientId,
                BaseProtocolId = x.BaseProtocolId,
                Name = x.Name,
                Description = x.Description,
                Status = x.Status
            };
        }

        public async Task<CustomizedArvProtocolDto> CreateAsync(CreateCustomizedArvProtocolDto dto)
        {
            var entity = new CustomizedArvProtocol
            {
                DoctorId = dto.DoctorId,
                PatientId = dto.PatientId,
                BaseProtocolId = dto.BaseProtocolId,
                Name = dto.Name,
                Description = dto.Description,
                Status = dto.Status
            };

            _context.CustomizedARVProtocols.Add(entity);
            await _context.SaveChangesAsync();

            return new CustomizedArvProtocolDto
            {
                CustomProtocolId = entity.CustomProtocolId,
                DoctorId = entity.DoctorId,
                PatientId = entity.PatientId,
                BaseProtocolId = entity.BaseProtocolId,
                Name = entity.Name,
                Description = entity.Description,
                Status = entity.Status
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateCustomizedArvProtocolDto dto)
        {
            var entity = await _context.CustomizedARVProtocols.FindAsync(id);
            if (entity == null) return false;

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Status = dto.Status;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.CustomizedARVProtocols.FindAsync(id);
            if (entity == null) return false;

            entity.Status = "DELETED";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
