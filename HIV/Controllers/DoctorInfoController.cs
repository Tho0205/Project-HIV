using HIV.DTOs;
using HIV.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HIV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Staff,Admin")]
    public class DoctorInfoController : ControllerBase
    {
        private readonly IDoctorInfoService _service;

        public DoctorInfoController(IDoctorInfoService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous] // Allow anyone to see doctor list
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _service.GetAllAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}", success = false });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous] // Allow anyone to see doctor details
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var data = await _service.GetByIdAsync(id);
                if (data == null) return NotFound(new { message = "Không tìm thấy bác sĩ", success = false });
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}", success = false });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDoctorInfoDto dto)
        {
            try
            {
                // Validate model
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new
                    {
                        message = "Dữ liệu không hợp lệ",
                        errors = errors,
                        success = false
                    });
                }

                var success = await _service.UpdateAsync(id, dto);
                if (success)
                {
                    return Ok(new { message = "Cập nhật thành công", success = true });
                }
                else
                {
                    return NotFound(new { message = "Không tìm thấy bác sĩ", success = false });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}", success = false });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _service.DeleteAsync(id);
                if (success)
                {
                    return Ok(new { message = "Xóa thành công", success = true });
                }
                return NotFound(new { message = "Không tìm thấy bác sĩ", success = false });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}", success = false });
            }
        }

        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveDoctors()
        {
            try
            {
                var data = await _service.GetAllAsync();
                var activeDoctors = data.Where(d => d.Status == "ACTIVE");
                return Ok(activeDoctors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}", success = false });
            }
        }

        [HttpPost("upload-avatar/{doctorId}")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> UploadAvatar(int doctorId, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "Không có file được tải lên", success = false });

                // Validate file type
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                    return BadRequest(new { message = "Chỉ chấp nhận file ảnh (JPEG, PNG, GIF)", success = false });

                // Validate file size (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                    return BadRequest(new { message = "Kích thước file không được vượt quá 5MB", success = false });

                // Use existing Uploads/Avatars folder
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Avatars");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                // Generate unique filename
                var fileExtension = Path.GetExtension(file.FileName);
                var fileName = $"doctor_{doctorId}_{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return the URL path
                var fileUrl = $"/Uploads/Avatars/{fileName}";
                return Ok(new { avatarUrl = fileUrl, success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi tải file: {ex.Message}", success = false });
            }
        }

        [HttpPost("sync-doctor-users")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> SyncDoctorUsers()
        {
            try
            {
                var result = await _service.SyncDoctorUsersAsync();
                return Ok(new
                {
                    message = "Đồng bộ hoàn tất",
                    newDoctorsAdded = result,
                    success = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi đồng bộ: {ex.Message}", success = false });
            }
        }
    }
}