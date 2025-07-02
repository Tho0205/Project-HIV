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

        /// <summary>
        /// Lấy danh sách bệnh nhân được phân công cho doctor
        /// </summary>
        [HttpGet("Patients")]
        [Authorize(Roles = "Doctor,Patient")]
        [ProducesResponseType(typeof(DoctorPatientsResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DoctorPatientsResponseDto>> GetDoctorPatients(
            [FromQuery] int doctorId,
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
                    doctorId, sortBy, order, page, pageSize);

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
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn" });
            }
        }

        /// <summary>
        /// Cập nhật thông tin bệnh nhân
        /// </summary>
        [HttpPut("UpdatePatient/{accountId}")]
        [Authorize(Roles = "Doctor")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> UpdatePatientInfo(
            int accountId,
            [FromBody] DoctorPatientUpdateDto dto)
        {
            try
            {
                if (accountId <= 0)
                {
                    return BadRequest(new { message = "AccountId không hợp lệ" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToList();
                    return BadRequest(new { message = "Dữ liệu không hợp lệ", errors });
                }

                // Add status to DTO if not provided
                if (string.IsNullOrEmpty(dto.Status))
                {
                    dto.Status = "ACTIVE";
                }

                var result = await _doctorPatientService.UpdatePatientInfoAsync(accountId, dto);

                if (!result)
                {
                    return NotFound(new { message = "Không tìm thấy bệnh nhân" });
                }

                return NoContent();
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Application error updating patient info");
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating patient info");
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn" });
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
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn" });
            }
        }

        /// <summary>
        /// Lấy lịch sử khám bệnh của bệnh nhân với doctor hiện tại
        /// </summary>
        [HttpGet("PatientHistory/{patientId}")]
        [Authorize(Roles = "Doctor,Patient")]
        [ProducesResponseType(typeof(PatientHistoryDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PatientHistoryDto>> GetPatientHistory(
            int patientId,
            [FromQuery] int doctorId)
        {
            try
            {
                if (patientId <= 0 || doctorId <= 0)
                {
                    return BadRequest(new { message = "PatientId và DoctorId không hợp lệ" });
                }

                // Kiểm tra quyền truy cập
                var hasAccess = await _doctorPatientService.CanDoctorAccessPatientAsync(doctorId, patientId);
                if (!hasAccess)
                {
                    return StatusCode(403, new { message = "Bạn không có quyền truy cập thông tin bệnh nhân này" });
                }

                var history = await _doctorPatientService.GetPatientHistoryAsync(patientId, doctorId);
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
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn" });
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
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn" });
            }
        }
    }
}