using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace HIV.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashBoardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashBoardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("TotalUsers")]
        public async Task<IActionResult> GetTotalUsers()
        {

            int total = await _context.Users
                .Where(x => x.Role == "Patient")
                .CountAsync();
            return Ok(total);
        }

        [HttpGet("TotalExams")]
        public async Task<IActionResult> GetTotalExams()
        {
            int total = await _context.Examinations.CountAsync();
            return Ok(total);
        }

        [HttpGet("TotalMedicalRecords")]
        public async Task<IActionResult> GetTotalMedicalRecords()
        {
            int total = await _context.MedicalRecords.CountAsync();
            return Ok(total);
        }

        [HttpGet("TotalARVProtocols")]
        public async Task<IActionResult> GetTotalARVProtocols()
        {
            int total = await _context.ARVProtocols.CountAsync();
            return Ok(total);
        }

        [HttpGet("PatientsByGender")]
        public async Task<IActionResult> GetPatientsByGender()
        {
            var grouped = await _context.Users
                .Where(x => x.Role == "Patient")
                .GroupBy(u => u.Gender)
                .Select(g => new { Gender = g.Key, Count = g.Count() })
                .ToListAsync();
            return Ok(grouped);
        }

        [HttpGet("ARVProtocolStats")]
        public async Task<IActionResult> GetARVProtocolStats()
        {
            var stats = await _context.CustomizedARVProtocols
                .Include (x => x.BaseProtocol)
                .GroupBy(p => p.BaseProtocol.Name)
                .Select(g => new { Protocol = g.Key, Count = g.Count() })
                .ToListAsync();
            return Ok(stats);
        }


        [HttpGet("NewUsersPerMonth")]
        public async Task<IActionResult> GetNewUsersPerMonth()
        {
            var raw = await _context.Users
                .Include(u => u.Account) 
                .Where(u => u.Role == "Patient" && u.Account != null)
                .GroupBy(u => new
                {
                    u.Account.CreatedAt.Year,
                    u.Account.CreatedAt.Month
                })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();

            var result = raw.Select(g => new 
            {
                Month = $"{g.Month}/{g.Year}",
                g.Count
            });

            return Ok(result);
        }


    }
}
