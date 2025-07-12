using HIV.DTOs.DoctorPatient;
using HIV.Interfaces;
using HIV.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HIV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorMangamentPatient _doctorPatientService;
        private readonly ILogger<DoctorController> _logger;

        public DoctorController(
            IDoctorMangamentPatient doctorPatientService,
            ILogger<DoctorController> logger)
        {
            _doctorPatientService = doctorPatientService;
            _logger = logger;
        }

        ///// <summary>
        ///// Lấy danh sách bệnh nhân được phân công cho doctor
        ///// </summary>
        //[HttpGet("Patients")]
        //[Authorize(Roles = "Doctor,Patient")]
        //[ProducesResponseType(typeof(DoctorPatientsResponseDto), 200)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(500)]
        //public async Task<ActionResult<DoctorPatientsResponseDto>> GetDoctorPatients(
        //    [FromQuery] int doctorId,
        //    [FromQuery] string sortBy = "full_name",
        //    [FromQuery] string order = "asc",
        //    [FromQuery] int page = 1,
        //    [FromQuery] int pageSize = 8)
        //{
        //    try
        //    {
        //        if (doctorId <= 0)
        //        {
        //            return BadRequest(new { message = "DoctorId không hợp lệ" });
        //        }

        //        if (page < 1) page = 1;
        //        if (pageSize < 1 || pageSize > 100) pageSize = 8;

        //        var result = await _doctorPatientService.GetDoctorPatientsAsync(
        //            doctorId, sortBy, order, page, pageSize);

        //        return Ok(result);
        //    }
        //    catch (ApplicationException ex)
        //    {
        //        _logger.LogError(ex, "Application error getting doctor patients");
        //        return StatusCode(500, new { message = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Unexpected error getting doctor patients");
        //        return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn" });
        //    }
        //}

        /// <summary>
        /// Lấy toàn bộ danh sách bệnh nhân trong hệ thống
        /// </summary>
        [HttpGet("AllPatients")]
        [Authorize(Roles = "Doctor,Admin")]
        [ProducesResponseType(typeof(DoctorPatientsResponseDto), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DoctorPatientsResponseDto>> GetAllPatients(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string sortBy = "full_name",
            [FromQuery] string order = "asc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var result = await _doctorPatientService.GetAllPatientsAsync(
                    searchTerm, sortBy, order, page, pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all patients");
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn" });
            }
        }

        /// <summary>
        /// Lấy danh sách bệnh nhân được phân công cho doctor với bộ lọc schedule
        /// </summary>
        [HttpGet("Patients")]
        [Authorize(Roles = "Doctor,Patient")]
        [ProducesResponseType(typeof(DoctorPatientsResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DoctorPatientsResponseDto>> GetDoctorPatients(
            [FromQuery] int doctorId,
            [FromQuery] DateTime? scheduleDate = null,
            [FromQuery] bool hasScheduleOnly = false,
            [FromQuery] string sortBy = "full_name",
            [FromQuery] string order = "asc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (doctorId <= 0)
                {
                    return BadRequest(new { message = "DoctorId không hợp lệ" });
                }

                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var result = await _doctorPatientService.GetDoctorPatientsAsync(
                    doctorId, scheduleDate, hasScheduleOnly, sortBy, order, page, pageSize);

                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Application error getting doctor patients");
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting doctor patients");
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn-6" });
            }
        }


        /// <summary>
        /// Lấy thống kê bệnh nhân của doctor
        /// </summary>
        [HttpGet("PatientStats/{doctorId}")]
        [Authorize(Roles = "Doctor,Patient")]
        [ProducesResponseType(typeof(DoctorPatientStatsDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DoctorPatientStatsDto>> GetPatientStats(int doctorId)
        {
            try
            {
                if (doctorId <= 0)
                {
                    return BadRequest(new { message = "DoctorId không hợp lệ" });
                }

                var stats = await _doctorPatientService.GetDoctorPatientStatsAsync(doctorId);
                return Ok(stats);
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Application error getting patient stats");
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting patient stats");
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn-5" });
            }
        }

        /// <summary>
        /// Lấy lịch sử khám bệnh của bệnh nhân với doctor hiện tại
        /// </summary>
        [HttpGet("PatientHistory/{patientId}")]
        [Authorize(Roles = "Doctor,Patient")]
        [ProducesResponseType(typeof(PatientHistoryDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PatientHistoryDto>> GetPatientHistory(int patientId)
        {
            try
            {
                if (patientId <= 0)
                {
                    return BadRequest(new { message = "PatientId không hợp lệ" });
                }

                // Gọi service không có doctorId
                var history = await _doctorPatientService.GetPatientHistoryAsync(patientId);
                return Ok(history);
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Application error getting patient history");
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting patient history");
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn-4" });
            }
        }

        /// <summary>
        /// Lấy chi tiết một bệnh nhân cụ thể
        /// </summary>
        [HttpGet("PatientDetail/{patientId}")]
        [Authorize(Roles = "Doctor,Patient")]
        [ProducesResponseType(typeof(DoctorPatientListDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DoctorPatientListDto>> GetPatientDetail(
            int patientId,
            [FromQuery] int doctorId)
        {
            try
            {
                if (patientId <= 0 || doctorId <= 0)
                {
                    return BadRequest(new { message = "PatientId và DoctorId không hợp lệ" });
                }

                var patient = await _doctorPatientService.GetPatientDetailAsync(patientId, doctorId);

                if (patient == null)
                {
                    return NotFound(new { message = "Không tìm thấy bệnh nhân hoặc bạn không có quyền truy cập" });
                }

                return Ok(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient detail");
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn-3" });
            }
        }

        ///// <summary>
        ///// Lấy danh sách bệnh nhân chưa có bác sĩ
        ///// </summary>
        //[HttpGet("AvailablePatients")]
        //[Authorize(Roles = "Doctor")]
        //[ProducesResponseType(typeof(List<DoctorPatientListDto>), 200)]
        //[ProducesResponseType(500)]
        //public async Task<ActionResult<List<DoctorPatientListDto>>> GetAvailablePatients()
        //{
        //    try
        //    {
        //        var patients = await _doctorPatientService.GetAvailablePatientsAsync();
        //        return Ok(patients);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting available patients");
        //        return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn" });
        //    }
        //}

        ///// <summary>
        ///// Thêm bệnh nhân vào danh sách quản lý của bác sĩ
        ///// </summary>
        //[HttpPost("AssignPatient")]
        //[Authorize(Roles = "Doctor")]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(500)]
        //public async Task<ActionResult> AssignPatientToDoctor(
        //    [FromBody] AssignPatientDto dto)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(new { message = "Dữ liệu không hợp lệ" });
        //        }

        //        var result = await _doctorPatientService.AssignPatientToDoctorAsync(dto.DoctorId, dto.PatientId);

        //        if (!result)
        //        {
        //            return BadRequest(new { message = "Không thể thêm bệnh nhân" });
        //        }

        //        return NoContent();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error assigning patient to doctor");
        //        return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn" });
        //    }
        //}
    }
}