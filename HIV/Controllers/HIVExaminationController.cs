using HIV.DTOs;
using HIV.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HIV.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HIVExaminationController : ControllerBase
    {
        private readonly IHIVExaminationService _service;
        private readonly ILogger<HIVExaminationController> _logger;

        public HIVExaminationController(
            IHIVExaminationService service,
            ILogger<HIVExaminationController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Get all patients with exam count
        /// </summary>
        [HttpGet("patients")]
        [Authorize(Roles = "Staff,Manager,Doctor,Patient")]
        public async Task<ActionResult<BaseResponseDTO<List<PatientListDTO>>>> GetPatients(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var patients = await _service.GetPatientsAsync(page, pageSize);
                return Ok(BaseResponseDTO<List<PatientListDTO>>.Ok(patients));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patients");
                return StatusCode(500, BaseResponseDTO<List<PatientListDTO>>.Fail("Internal server error"));
            }
        }

        /// <summary>
        /// Get patient examinations
        /// </summary>
        [HttpGet("patient/{patientId}/examinations")]
        [Authorize(Roles = "Staff,Manager,Doctor,Patient")]
        public async Task<ActionResult<BaseResponseDTO<List<ExaminationDTO>>>> GetPatientExaminations(int patientId)
        {
            try
            {
                var examinations = await _service.GetPatientExaminationsAsync(patientId);
                return Ok(BaseResponseDTO<List<ExaminationDTO>>.Ok(examinations));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient examinations");
                return StatusCode(500, BaseResponseDTO<List<ExaminationDTO>>.Fail("Internal server error"));
            }
        }

        /// <summary>
        /// Create or Update examination
        /// </summary>
        [HttpPost("save")]
        [Authorize(Roles = "Staff,Manager,Doctor,Patient")]
        public async Task<ActionResult<BaseResponseDTO<ExaminationDTO>>> SaveExamination(
            [FromBody] ExaminationFormDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(BaseResponseDTO<ExaminationDTO>.Fail("Invalid data"));

                var result = await _service.SaveExaminationAsync(dto);
                var message = dto.ExamId.HasValue ? "Updated successfully" : "Created successfully";

                return Ok(BaseResponseDTO<ExaminationDTO>.Ok(result, message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(BaseResponseDTO<ExaminationDTO>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving examination");
                return StatusCode(500, BaseResponseDTO<ExaminationDTO>.Fail("Internal server error"));
            }
        }

        /// <summary>
        /// Delete examination (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff,Manager,Doctor,Patient")]
        public async Task<ActionResult<BaseResponseDTO<bool>>> DeleteExamination(int id)
        {
            try
            {
                var result = await _service.DeleteExaminationAsync(id);
                if (!result)
                    return NotFound(BaseResponseDTO<bool>.Fail("Examination not found"));

                return Ok(BaseResponseDTO<bool>.Ok(true, "Deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting examination");
                return StatusCode(500, BaseResponseDTO<bool>.Fail("Internal server error"));
            }
        }

        /// <summary>
        /// Get doctors for dropdown
        /// </summary>
        [HttpGet("doctors")]
        [Authorize(Roles = "Staff,Manager,Doctor,Patient")]
        public async Task<ActionResult<BaseResponseDTO<List<object>>>> GetDoctors()
        {
            try
            {
                var doctors = await _service.GetDoctorsAsync();
                return Ok(BaseResponseDTO<List<object>>.Ok(doctors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctors");
                return StatusCode(500, BaseResponseDTO<List<object>>.Fail("Internal server error"));
            }
        }
    }
}