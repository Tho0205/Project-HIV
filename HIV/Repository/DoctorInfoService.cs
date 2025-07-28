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
            // First, sync any new doctor users
            await SyncDoctorUsersAsync();

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

        public async Task<bool> UpdateAsync(int doctorId, UpdateDoctorInfoDto dto)
        {
            try
            {
                var entity = await _context.DoctorInfos.FindAsync(doctorId);
                if (entity == null) return false;

                // Update fields
                entity.Degree = dto.Degree ?? entity.Degree;
                entity.Specialization = dto.Specialization ?? entity.Specialization;
                entity.ExperienceYears = dto.ExperienceYears ?? entity.ExperienceYears;
                entity.DoctorAvatar = dto.DoctorAvatar ?? entity.DoctorAvatar;
                entity.Status = dto.Status ?? entity.Status;

                _context.DoctorInfos.Update(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log error if needed
                throw new Exception($"Error updating doctor info: {ex.Message}");
            }
        }

        public async Task<bool> DeleteAsync(int doctorId)
        {
            var entity = await _context.DoctorInfos.FindAsync(doctorId);
            if (entity == null) return false;

            entity.Status = "DELETED";
            await _context.SaveChangesAsync();
            return true;
        }

        // Auto-sync method to add users with Doctor role to DoctorInfo table
        public async Task<int> SyncDoctorUsersAsync()
        {
            try
            {
                // Find all users with Doctor role who don't have DoctorInfo yet
                var doctorUsersWithoutInfo = await _context.Users
                    .Where(u => u.Role != null && u.Role.ToLower().Contains("doctor"))
                    .Where(u => u.Status == "ACTIVE")
                    .Where(u => !_context.DoctorInfos.Any(di => di.DoctorId == u.UserId))
                    .ToListAsync();

                if (!doctorUsersWithoutInfo.Any())
                    return 0;

                // Create DoctorInfo entries for these users
                var newDoctorInfos = doctorUsersWithoutInfo.Select(user => new DoctorInfo
                {
                    DoctorId = user.UserId,
                    Degree = null, // Will be updated later by staff
                    Specialization = null, // Will be updated later by staff
                    ExperienceYears = null, // Will be updated later by staff
                    DoctorAvatar = user.UserAvatar ?? "/images/default-doctor.png", // Use user avatar or default
                    Status = "ACTIVE"
                }).ToList();

                _context.DoctorInfos.AddRange(newDoctorInfos);
                await _context.SaveChangesAsync();

                return newDoctorInfos.Count;
            }
            catch (Exception ex)
            {
                // Log error if needed
                throw new Exception($"Error syncing doctor users: {ex.Message}");
            }
        }
    }
}