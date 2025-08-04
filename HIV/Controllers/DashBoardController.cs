using Microsoft.AspNetCore.Authorization;
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


        //Tổng số bệnh nhân (Role == "Patient")
        [HttpGet("TotalUsers")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetTotalUsers()
        {

            int total = await _context.Users
                .Where(x => x.Role == "Patient")
                .CountAsync();
            return Ok(total);
        }


        //	Phân tích dịch tễ học
        [HttpGet("PatientsByGender")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetPatientsByGender()
        {
            var grouped = await _context.Users
                .Where(x => x.Role == "Patient")
                .GroupBy(u => u.Gender)
                .Select(g => new { Gender = g.Key, Count = g.Count() })
                .ToListAsync();
            return Ok(grouped);
        }


        //Tổng số lượt khám theo giới tính
        [HttpGet("PatientsByAgeGroup")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetPatientsByAgeGroup()
        {
            var today = DateTime.Today;

            var patients = await _context.Users
                .Where(x => x.Role == "Patient" && x.Birthdate != null)
                .ToListAsync(); 

            var grouped = patients
                .Select(u => {
                    var age = today.Year - u.Birthdate.Value.Year;
                    if (u.Birthdate.Value > DateOnly.FromDateTime(today.AddYears(-age))) age--; 
                    return new { Age = age };
                })
                .GroupBy(x =>
                    x.Age < 15 ? "Dưới 15" :
                    x.Age <= 24 ? "15-24" :
                    x.Age <= 49 ? "25-49" : ">=50"
                )
                .Select(g => new { AgeGroup = g.Key, Count = g.Count() })
                .ToList();

            return Ok(grouped);
        }


        //Theo dõi xu hướng tiếp nhận bệnh nhân
        [HttpGet("NewUsersPerMonth")]
        [Authorize(Roles = "Staff,Manager")]
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

        //Tổng số lượt khám
        [HttpGet("TotalExams")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetTotalExams()
        {
            int total = await _context.Examinations.CountAsync();
            return Ok(total);
        }



        // Thống kê số lượng bệnh nhân theo phác đồ ARV
        [HttpGet("ARVProtocolStats")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetARVProtocolStats()
        {
            var stats = await _context.CustomizedARVProtocols
                .Include(x => x.BaseProtocol)
                .GroupBy(p => p.BaseProtocol.Name)
                .Select(g => new { Protocol = g.Key, Count = g.Count() })
                .ToListAsync();
            return Ok(stats);
        }

        // Thống kê số lượng phác đồ ARV
        [HttpGet("TotalARVProtocols")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetTotalARVProtocols()
        {
            int total = await _context.ARVProtocols.CountAsync();
            return Ok(total);
        }


        // Thống kê số lượng bệnh nhân theo phác đồ ARV
        [HttpGet("TotalMedicalRecords")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetTotalMedicalRecords()
        {
            int total = await _context.MedicalRecords.CountAsync();
            return Ok(total);
        }




        // Thống kê số lượng lịch hẹn theo tháng
        [HttpGet("AppointmentsPerMonth")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetAppointmentsPerMonth()
        {
            // Bước 1: Truy vấn dữ liệu gộp theo năm/tháng (nhưng chưa dùng string.Format)
            var rawData = await _context.Appointments
                .GroupBy(a => new { a.AppointmentDate.Year, a.AppointmentDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();

            // Bước 2: Format tên tháng trong C#
            var result = rawData.Select(g => new
            {
                Month = $"{g.Month}/{g.Year}",
                g.Count
            });

            return Ok(result);
        }


        // Thống kê số lượng lịch hẹn theo bác sĩ
        [HttpGet("AppointmentsByDoctor")]
        [Authorize(Roles = "Manager, Staff")]
        public async Task<IActionResult> GetAppointmentsByDoctor()
        {
            var grouped = await _context.Appointments
                .GroupBy(a => a.Doctor.FullName)
                .Select(g => new {
                    Doctor = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return Ok(grouped);
        }

    }
}
