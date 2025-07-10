using HIV.DTOs.DTOARVs;
using HIV.Interfaces.ARVinterfaces;
using Microsoft.AspNetCore.Mvc;

namespace HIV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomArvProtocolsController : ControllerBase
    {
        private readonly ICustomizedArvProtocolService _protocolService;

        public CustomArvProtocolsController(ICustomizedArvProtocolService protocolService)
        {
            _protocolService = protocolService;
        }

        /// <summary>
        /// Get all active patients with their protocols for a specific doctor
        /// </summary>
        /// <param name="doctorId">The ID of the doctor</param>
        /// <returns>List of patients with their protocols</returns>
        [HttpGet("doctor/{doctorId}/patients")]
        public async Task<ActionResult<List<PatientWithProtocolDto>>> GetPatientsWithProtocols(int doctorId)
        {
            var result = await _protocolService.GetPatientsWithProtocolsAsync(doctorId);
            return Ok(result);
        }

        /// <summary>
        /// Get the current active protocol for a patient
        /// </summary>
        /// <param name="patientId">The ID of the patient</param>
        /// <returns>The patient's current protocol details</returns>
        [HttpGet("patient/{patientId}/current-protocol")]
        public async Task<ActionResult<FullCustomProtocolDto>> GetPatientCurrentProtocol(int patientId)
        {
            var protocol = await _protocolService.GetPatientCurrentProtocolAsync(patientId);

            if (protocol == null)
            {
                return NotFound($"No active protocol found for patient with ID {patientId}");
            }

            return Ok(protocol);
        }

        /// <summary>
        /// Create a new custom protocol for a patient
        /// </summary>
        /// <param name="doctorId">The ID of the creating doctor</param>
        /// <param name="patientId">The ID of the patient</param>
        /// <param name="request">The protocol creation request</param>
        /// <returns>The created protocol details</returns>
        [HttpPost("doctor/{doctorId}/patient/{patientId}")]
        public async Task<ActionResult<FullCustomProtocolDto>> CreateCustomProtocol(
            int doctorId,
            int patientId,
            [FromBody] CreateCustomProtocolRequest request)
        {
            try
            {
                var result = await _protocolService.CreateCustomProtocolAsync(doctorId, patientId, request);
                return CreatedAtAction(
                    nameof(GetPatientCurrentProtocol),
                    new { patientId = patientId },
                    result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the protocol: {ex.Message}");
            }
        }

        /// <summary>
        /// Update a patient's protocol (switch to standard or activate existing custom)
        /// </summary>
        /// <param name="patientId">The ID of the patient</param>
        /// <param name="request">The update request</param>
        /// <returns>True if successful</returns>
        [HttpPut("patient/{patientId}/update-protocol")]
        public async Task<IActionResult> UpdatePatientProtocol(
            int patientId,
            [FromBody] UpdatePatientProtocolRequest request)
        {
            try
            {
                var success = await _protocolService.UpdatePatientProtocolAsync(patientId, request);

                if (!success)
                {
                    return BadRequest("Failed to update patient protocol. Check if the protocol exists.");
                }

                return Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the protocol: {ex.Message}");
            }
        }

        /// <summary>
        /// Get the protocol history for a patient
        /// </summary>
        /// <param name="patientId">The ID of the patient</param>
        /// <returns>List of all protocols the patient has had</returns>
        [HttpGet("patient/{patientId}/history")]
        public async Task<ActionResult<List<FullCustomProtocolDto>>> GetPatientProtocolHistory(int patientId)
        {
            var history = await _protocolService.GetPatientProtocolHistoryAsync(patientId);
            return Ok(history);
        }
    }
}