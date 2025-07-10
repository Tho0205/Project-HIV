
using DemoSWP391.Services;
using HIV.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HIV.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        /// <summary>
        /// Lấy lịch làm việc của bác sĩ (Doctor Feature)
        /// </summary>
        [HttpGet("doctor/{doctorId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<ScheduleDto>>>> GetDoctorSchedules(
            int doctorId,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var schedules = await _scheduleService.GetDoctorSchedulesAsync(doctorId, fromDate, toDate);
                return Ok(ApiResponse<List<ScheduleDto>>.SuccessResult(schedules, "Lấy lịch làm việc thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<ScheduleDto>>.ErrorResult("Lỗi server: " + ex.Message));
            }
        }

        /// <summary>
        /// Lấy chi tiết một lịch làm việc
        /// </summary>
        [HttpGet("{scheduleId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<ApiResponse<ScheduleDto>>> GetScheduleById(int scheduleId)
        {
            try
            {
                var schedule = await _scheduleService.GetScheduleByIdAsync(scheduleId);
                if (schedule == null)
                    return NotFound(ApiResponse<ScheduleDto>.ErrorResult("Không tìm thấy lịch làm việc"));

                return Ok(ApiResponse<ScheduleDto>.SuccessResult(schedule, "Lấy chi tiết lịch làm việc thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ScheduleDto>.ErrorResult("Lỗi server: " + ex.Message));
            }
        }

        /// <summary>
        /// Tạo lịch làm việc mới (Doctor Feature)
        /// </summary>
        [HttpPost]
        //[AllowAnonymous]
        [Authorize(Roles = "Doctor,Staff")]
        public async Task<ActionResult<ApiResponse<ScheduleDto>>> CreateSchedule([FromBody] CreateScheduleDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(ApiResponse<ScheduleDto>.ErrorResult("Dữ liệu không hợp lệ", errors));
                }

                var result = await _scheduleService.CreateScheduleAsync(dto);
                return CreatedAtAction(nameof(GetScheduleById), new { scheduleId = result.ScheduleId },
                    ApiResponse<ScheduleDto>.SuccessResult(result, "Tạo lịch làm việc thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ScheduleDto>.ErrorResult("Lỗi server: " + ex.Message));
            }
        }

        /// <summary>
        /// Cập nhật lịch làm việc (Doctor Feature)
        /// </summary>
        [HttpPut("{scheduleId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<ApiResponse<ScheduleDto>>> UpdateSchedule(int scheduleId, [FromBody] UpdateScheduleDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(ApiResponse<ScheduleDto>.ErrorResult("Dữ liệu không hợp lệ", errors));
                }

                var result = await _scheduleService.UpdateScheduleAsync(scheduleId, dto);
                if (result == null)
                    return NotFound(ApiResponse<ScheduleDto>.ErrorResult("Không tìm thấy lịch làm việc"));

                return Ok(ApiResponse<ScheduleDto>.SuccessResult(result, "Cập nhật lịch làm việc thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ScheduleDto>.ErrorResult("Lỗi server: " + ex.Message));
            }
        }

        /// <summary>
        /// Xóa lịch làm việc (Doctor Feature)
        /// </summary>
        [HttpDelete("{scheduleId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteSchedule(int scheduleId)
        {
            try
            {
                var result = await _scheduleService.DeleteScheduleAsync(scheduleId);
                if (!result)
                    return BadRequest(ApiResponse<bool>.ErrorResult("Không thể xóa lịch làm việc (có thể đã có cuộc hẹn)"));

                return Ok(ApiResponse<bool>.SuccessResult(true, "Xóa lịch làm việc thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.ErrorResult("Lỗi server: " + ex.Message));
            }
        }

        /// <summary>
        /// Lấy danh sách lịch trống (Patient/Staff Feature)
        /// </summary>
        [HttpGet("available")]
        [Authorize(Roles = "Patient,Staff")]
        public async Task<ActionResult<ApiResponse<List<ScheduleDto>>>> GetAvailableSchedules([FromQuery] DateTime? date = null)
        {
            try
            {
                var schedules = await _scheduleService.GetAvailableSchedulesAsync(date);
                return Ok(ApiResponse<List<ScheduleDto>>.SuccessResult(schedules, "Lấy danh sách lịch trống thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<ScheduleDto>>.ErrorResult("Lỗi server: " + ex.Message));
            }
        }
        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<ScheduleDto>>>> GetActiveSchedules(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var schedules = await _scheduleService.GetActiveSchedulesAsync(fromDate, toDate);
                return Ok(ApiResponse<List<ScheduleDto>>.SuccessResult(schedules, "Lấy danh sách lịch làm việc hoạt động thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<ScheduleDto>>.ErrorResult("Lỗi server: " + ex.Message));
            }
        }

    }
}