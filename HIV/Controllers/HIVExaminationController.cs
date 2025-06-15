using HIV.DTOs;
using HIV.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HIV.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HIVExaminationController : ControllerBase
    {
        private readonly IHIVExaminationService _hivExaminationService;
        private readonly ILogger<HIVExaminationController> _logger;

        public HIVExaminationController(
            IHIVExaminationService hivExaminationService,
            ILogger<HIVExaminationController> logger)
        {
            _hivExaminationService = hivExaminationService;
            _logger = logger;
        }

        /// <summary>
        /// Tạo kết quả xét nghiệm HIV mới (Staff Feature)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponseDTO<HIVExaminationResponseDTO>>> CreateHIVExamination(
            [FromBody] CreateHIVExaminationDTO createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponseDTO<HIVExaminationResponseDTO>.ErrorResult(
                        "Dữ liệu không hợp lệ", errors));
                }

                var result = await _hivExaminationService.CreateHIVExaminationAsync(createDto);

                _logger.LogInformation("Staff tạo kết quả xét nghiệm HIV thành công - ExamId: {ExamId}", result.ExamId);

                return CreatedAtAction(
                    nameof(GetHIVExaminationById),
                    new { id = result.ExamId },
                    ApiResponseDTO<HIVExaminationResponseDTO>.SuccessResult(result, "Tạo kết quả xét nghiệm thành công"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Validation error: {Message}", ex.Message);
                return BadRequest(ApiResponseDTO<HIVExaminationResponseDTO>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo kết quả xét nghiệm HIV");
                return StatusCode(500, ApiResponseDTO<HIVExaminationResponseDTO>.ErrorResult(
                    "Đã xảy ra lỗi trong hệ thống. Vui lòng thử lại sau."));
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết kết quả xét nghiệm HIV
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDTO<HIVExaminationResponseDTO>>> GetHIVExaminationById(int id)
        {
            try
            {
                var result = await _hivExaminationService.GetHIVExaminationByIdAsync(id);
                return Ok(ApiResponseDTO<HIVExaminationResponseDTO>.SuccessResult(result, "Lấy thông tin thành công"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseDTO<HIVExaminationResponseDTO>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin xét nghiệm HIV - ID: {ExamId}", id);
                return StatusCode(500, ApiResponseDTO<HIVExaminationResponseDTO>.ErrorResult(
                    "Đã xảy ra lỗi trong hệ thống"));
            }
        }

        /// <summary>
        /// Lấy lịch sử xét nghiệm HIV của bệnh nhân
        /// </summary>
        [HttpGet("patient/{patientId}/history")]
        public async Task<ActionResult<ApiResponseDTO<List<HIVExaminationResponseDTO>>>> GetPatientHistory(int patientId)
        {
            try
            {
                var history = await _hivExaminationService.GetPatientHIVExaminationHistoryAsync(patientId);
                return Ok(ApiResponseDTO<List<HIVExaminationResponseDTO>>.SuccessResult(
                    history, "Lấy lịch sử xét nghiệm thành công"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch sử xét nghiệm của bệnh nhân - PatientId: {PatientId}", patientId);
                return StatusCode(500, ApiResponseDTO<List<HIVExaminationResponseDTO>>.ErrorResult(
                    "Đã xảy ra lỗi trong hệ thống"));
            }
        }

        /// <summary>
        /// Cập nhật kết quả xét nghiệm HIV
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDTO<HIVExaminationResponseDTO>>> UpdateHIVExamination(
            int id, [FromBody] UpdateHIVExaminationDTO updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponseDTO<HIVExaminationResponseDTO>.ErrorResult(
                        "Dữ liệu không hợp lệ", errors));
                }

                var result = await _hivExaminationService.UpdateHIVExaminationAsync(id, updateDto);

                _logger.LogInformation("Cập nhật kết quả xét nghiệm HIV thành công - ExamId: {ExamId}", id);

                return Ok(ApiResponseDTO<HIVExaminationResponseDTO>.SuccessResult(result, "Cập nhật thành công"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseDTO<HIVExaminationResponseDTO>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật kết quả xét nghiệm HIV - ExamId: {ExamId}", id);
                return StatusCode(500, ApiResponseDTO<HIVExaminationResponseDTO>.ErrorResult(
                    "Đã xảy ra lỗi trong hệ thống"));
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả kết quả xét nghiệm (có phân trang)
        /// </summary>
        [HttpGet("all")]
        public async Task<ActionResult<ApiResponseDTO<List<HIVExaminationSummaryDTO>>>> GetAllExaminations(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var examinations = await _hivExaminationService.GetAllHIVExaminationsAsync(page, pageSize);
                return Ok(ApiResponseDTO<List<HIVExaminationSummaryDTO>>.SuccessResult(
                    examinations, "Lấy danh sách thành công"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách tất cả xét nghiệm HIV");
                return StatusCode(500, ApiResponseDTO<List<HIVExaminationSummaryDTO>>.ErrorResult(
                    "Đã xảy ra lỗi trong hệ thống"));
            }
        }

        /// <summary>
        /// Lấy danh sách bác sĩ để chọn trong form
        /// </summary>
        [HttpGet("doctors")]
        public async Task<ActionResult<ApiResponseDTO<List<DoctorSimpleDTO>>>> GetDoctorsForSelection()
        {
            try
            {
                var doctors = await _hivExaminationService.GetDoctorsForSelectionAsync();
                return Ok(ApiResponseDTO<List<DoctorSimpleDTO>>.SuccessResult(
                    doctors, "Lấy danh sách bác sĩ thành công"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách bác sĩ");
                return StatusCode(500, ApiResponseDTO<List<DoctorSimpleDTO>>.ErrorResult(
                    "Đã xảy ra lỗi trong hệ thống"));
            }
        }

        /// <summary>
        /// Xóa kết quả xét nghiệm HIV (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDTO<bool>>> DeleteHIVExamination(int id)
        {
            try
            {
                var result = await _hivExaminationService.DeleteHIVExaminationAsync(id);

                if (!result)
                {
                    return NotFound(ApiResponseDTO<bool>.ErrorResult("Không tìm thấy kết quả xét nghiệm"));
                }

                _logger.LogInformation("Xóa kết quả xét nghiệm HIV thành công - ExamId: {ExamId}", id);

                return Ok(ApiResponseDTO<bool>.SuccessResult(true, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa kết quả xét nghiệm HIV - ExamId: {ExamId}", id);
                return StatusCode(500, ApiResponseDTO<bool>.ErrorResult(
                    "Đã xảy ra lỗi trong hệ thống"));
            }
        }
    }
}