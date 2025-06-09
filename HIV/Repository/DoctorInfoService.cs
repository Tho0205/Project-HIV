using HIV.DTOs;
using HIV.Interfaces;
using HIV.Models;
using Microsoft.EntityFrameworkCore;

namespace HIV.Repository
{
    public class DoctorInfoService : IDoctorInfoService
    {
        private readonly AppDbContext _context;

        public DoctorInfoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DoctorInfoDto>> GetAllAsync()
        {
            return await _context.DoctorInfos
                .Include(d => d.Doctor)
                .Select(d => new DoctorInfoDto
                {
                    DoctorId = d.DoctorId,
                    Degree = d.Degree,
                    Specialization = d.Specialization,
                    ExperienceYears = d.ExperienceYears,
                    DoctorAvatar = d.DoctorAvatar,
                    Status = d.Status,
                    DoctorName = d.Doctor != null ? d.Doctor.FullName : null
                })
                .ToListAsync();
        }

        public async Task<DoctorInfoDto?> GetByIdAsync(int doctorId)
        {
            var d = await _context.DoctorInfos
                .Include(x => x.Doctor)
                .FirstOrDefaultAsync(x => x.DoctorId == doctorId);

            if (d == null) return null;

            return new DoctorInfoDto
            {
                DoctorId = d.DoctorId,
                Degree = d.Degree,
                Specialization = d.Specialization,
                ExperienceYears = d.ExperienceYears,
                DoctorAvatar = d.DoctorAvatar,
                Status = d.Status,
                DoctorName = d.Doctor?.FullName
            };
        }

        public async Task<DoctorInfoDto> CreateAsync(CreateDoctorInfoDto dto)
        {
            var entity = new DoctorInfo
            {
                DoctorId = dto.DoctorId,
                Degree = dto.Degree,
                Specialization = dto.Specialization,
                ExperienceYears = dto.ExperienceYears,
                DoctorAvatar = dto.DoctorAvatar,
                Status = "ACTIVE"
            };

            _context.DoctorInfos.Add(entity);
            await _context.SaveChangesAsync();

            return new DoctorInfoDto
            {
                DoctorId = entity.DoctorId,
                Degree = entity.Degree,
                Specialization = entity.Specialization,
                ExperienceYears = entity.ExperienceYears,
                DoctorAvatar = entity.DoctorAvatar,
                Status = entity.Status,
                DoctorName = null // Optional: you can query user table here if needed
            };
        }

        public async Task<bool> UpdateAsync(int doctorId, UpdateDoctorInfoDto dto)
        {
            var entity = await _context.DoctorInfos.FindAsync(doctorId);
            if (entity == null) return false;

            entity.Degree = dto.Degree;
            entity.Specialization = dto.Specialization;
            entity.ExperienceYears = dto.ExperienceYears;
            entity.DoctorAvatar = dto.DoctorAvatar;
            entity.Status = dto.Status;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int doctorId)
        {
            var entity = await _context.DoctorInfos.FindAsync(doctorId);
            if (entity == null) return false;

            entity.Status = "DELETED";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
