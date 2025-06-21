using HIV.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HIV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StaffController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("Patient")]
        public async Task<ActionResult<IEnumerable<DTOGetPatient>>> GetAllPatient(
            string sortBy = "full_name",
            string order = "asc",
            int page = 1,
            int pageSize = 8)
        {
            var query = _context.Accounts
                .Include(a => a.User)
                .Where(a => a.User.Role == "Patient" && (a.User.Status == "ACTIVE" || a.User.Status == "INACTIVE"))
                .AsQueryable();

            // sort full_name or created_at
            query = (sortBy.ToLower(), order.ToLower()) switch
            {
                ("full_name", "asc") => query.OrderBy(a => a.User.FullName),
                ("full_name", "desc") => query.OrderByDescending(a => a.User.FullName),
                ("created_at", "asc") => query.OrderBy(a => a.CreatedAt),
                ("created_at", "desc") => query.OrderByDescending(a => a.CreatedAt),
                _ => query.OrderBy(a => a.User.FullName)
            };

            // Lấy tổng số lượng bản ghi
            var totalCount = await query.CountAsync();

            // Phân trang
            var patients = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(account => new DTOGetPatient
                {
                    AccountId = account.AccountId,
                    email = account.Email,
                    created_at = (DateTime)account.CreatedAt,
                    full_name = account.User.FullName,
                    phone = account.User.Phone,
                    gender = account.User.Gender,
                    birthdate = account.User.Birthdate,
                    address = account.User.Address,
                    UserAvatar = account.User.UserAvatar,
                    status = account.Status
                })
                .ToListAsync();

            // Trả về 1 cái dùng để phần trang
            return Ok(new
            {
                total = totalCount,
                page,
                pageSize,
                data = patients
            });
        }



        [HttpPut("Staff-Update/{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<DTOUpdate>> StaffUpdateInfo(int id, [FromBody] DTOStaffUpdate updateinfo)    
        {
            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }
            account.Email = updateinfo.email;
            account.User.FullName = updateinfo.full_name;
            account.User.Gender = updateinfo.gender;
            account.User.Phone = updateinfo.phone;
            account.User.Birthdate = updateinfo.birthdate;
            account.User.Role = updateinfo.role;
            account.User.Address = updateinfo.address;
            account.User.Status = updateinfo.Status;
            account.Status = updateinfo.Status;

            await _context.SaveChangesAsync();

            return NoContent();
        }

       
    }
}
