
using DemoSWP391.Services;
using HIV.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HIV.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExaminationController : ControllerBase
    {
        private readonly IExaminationService _examinationService;

        public ExaminationController(IExaminationService examinationService)
        {
            _examinationService = examinationService;
        }

        /// <summary>
        /// Lấy tất cả kết quả xét nghiệm của một bệnh nhân (Patient Feature)
        /// </summary>
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<ApiResponse<List<ExaminationResultDto>>>> GetPatientExaminationResults(int patientId)
        {
            try
            {
                var results = await _examinationService.GetPatientExaminationResultsAsync(patientId);
                return Ok(ApiResponse<List<ExaminationResultDto>>.SuccessResult(results, "Lấy kết quả xét nghiệm thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<ExaminationResultDto>>.ErrorResult("Lỗi server: " + ex.Message));
            }
        }

        /// <summary>
        /// Lấy chi tiết một kết quả xét nghiệm
        /// </summary>
        [HttpGet("{examId}")]
        public async Task<ActionResult<ApiResponse<ExaminationResultDto>>> GetExaminationById(int examId)
        {
            try
            {
                var result = await _examinationService.GetExaminationByIdAsync(examId);
                if (result == null)
                    return NotFound(ApiResponse<ExaminationResultDto>.ErrorResult("Không tìm thấy kết quả xét nghiệm"));

                return Ok(ApiResponse<ExaminationResultDto>.SuccessResult(result, "Lấy chi tiết kết quả xét nghiệm thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ExaminationResultDto>.ErrorResult("Lỗi server: " + ex.Message));
            }
        }

        /// <summary>
        /// Tạo kết quả xét nghiệm mới (Staff/Doctor Feature)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ExaminationResultDto>>> CreateExamination([FromBody] CreateExaminationDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(ApiResponse<ExaminationResultDto>.ErrorResult("Dữ liệu không hợp lệ", errors));
                }

                var result = await _examinationService.CreateExaminationAsync(dto);
                return CreatedAtAction(nameof(GetExaminationById), new { examId = result.ExamId },
                    ApiResponse<ExaminationResultDto>.SuccessResult(result, "Tạo kết quả xét nghiệm thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ExaminationResultDto>.ErrorResult("Lỗi server: " + ex.Message));
            }
        }

        /// <summary>
        /// Cập nhật kết quả xét nghiệm (Staff/Doctor Feature)
        /// </summary>
        [HttpPut("{examId}")]
        public async Task<ActionResult<ApiResponse<ExaminationResultDto>>> UpdateExamination(int examId, [FromBody] UpdateExaminationDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(ApiResponse<ExaminationResultDto>.ErrorResult("Dữ liệu không hợp lệ", errors));
                }

                var result = await _examinationService.UpdateExaminationAsync(examId, dto);
                if (result == null)
                    return NotFound(ApiResponse<ExaminationResultDto>.ErrorResult("Không tìm thấy kết quả xét nghiệm"));

                return Ok(ApiResponse<ExaminationResultDto>.SuccessResult(result, "Cập nhật kết quả xét nghiệm thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ExaminationResultDto>.ErrorResult("Lỗi server: " + ex.Message));
            }
        }

        /// <summary>
        /// Xóa kết quả xét nghiệm (Admin Feature)
        /// </summary>
        [HttpDelete("{examId}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteExamination(int examId)
        {
            try
            {
                var result = await _examinationService.DeleteExaminationAsync(examId);
                if (!result)
                    return NotFound(ApiResponse<bool>.ErrorResult("Không tìm thấy kết quả xét nghiệm"));

                return Ok(ApiResponse<bool>.SuccessResult(true, "Xóa kết quả xét nghiệm thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.ErrorResult("Lỗi server: " + ex.Message));
            }
        }
    }
}