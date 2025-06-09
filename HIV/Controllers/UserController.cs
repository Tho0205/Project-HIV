using HIV.Interfaces;
using HIV.Models;
using HIV.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HIV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ICommonOperation<User> _repo;
        private readonly AppDbContext _context; // THÊM CONTEXT

        public UserController(ICommonOperation<User> repo, AppDbContext context)
        {
            _repo = repo;
            _context = context; // INJECT CONTEXT
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _repo.GetAllAysnc());
        }

        // THÊM ENDPOINT MỚI CHO FRONTEND
        [HttpGet("patients-with-appointments")]
        public async Task<IActionResult> GetPatientsWithAppointments()
        {
            try
            {
                var patientsWithAppointments = await _context.Appointments
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.Account)
                    .Include(a => a.Doctor)
                    .Where(a => a.Status == "CONFIRMED" || a.Status == "COMPLETED" || a.Status == "Pending")
                    .Where(a => a.Patient.Role == "PATIENT")
                    .Select(a => new UserWithAppointmentDto
                    {
                        UserId = a.Patient.UserId,
                        FullName = a.Patient.FullName,
                        Email = a.Patient.Account.Email,
                        Phone = a.Patient.Phone,
                        AppointmentDate = a.AppointmentDate,
                        ConsultationType = a.AppoinmentType ?? "Trực Tiếp",
                        Gender = a.Patient.Gender,
                        DoctorName = a.Doctor.FullName,
                        AppointmentId = a.AppointmentId,
                        Role = a.Patient.Role
                    })
                    .Take(10)
                    .ToListAsync();

                var response = new ApiResponse<List<UserWithAppointmentDto>>
                {
                    Success = true,
                    Message = "Lấy danh sách thành công",
                    Data = patientsWithAppointments
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<List<UserWithAppointmentDto>>
                {
                    Success = false,
                    Message = "Lỗi server: " + ex.Message,
                    Data = null
                };
                return StatusCode(500, errorResponse);
            }
        }

        // THÊM ENDPOINT LẤY CHI TIẾT USER
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Account)
                    .Where(u => u.UserId == id)
                    .Select(u => new UserDetailDto
                    {
                        UserId = u.UserId,
                        FullName = u.FullName,
                        Birthdate = u.Birthdate,
                        Gender = u.Gender,
                        Email = u.Account.Email,
                        Phone = u.Phone,
                        Address = u.Address,
                        UserAvatar = u.UserAvatar,
                        Role = u.Role
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    var notFoundResponse = new ApiResponse<UserDetailDto>
                    {
                        Success = false,
                        Message = "Không tìm thấy người dùng"
                    };
                    return NotFound(notFoundResponse);
                }

                var response = new ApiResponse<UserDetailDto>
                {
                    Success = true,
                    Message = "Lấy thông tin thành công",
                    Data = user
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<UserDetailDto>
                {
                    Success = false,
                    Message = "Lỗi server: " + ex.Message
                };
                return StatusCode(500, errorResponse);
            }
        }
    }
}