using HIV.DTOs;
using HIV.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HIV.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordService _service;

        public MedicalRecordController(IMedicalRecordService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Staff,Manager,Doctor,Patient")]
        public async Task<IActionResult> GetAll()
        {
            var records = await _service.GetAllAsync();
            return Ok(records);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Staff,Manager,Doctor,Patient")]
        public async Task<IActionResult> GetById(int id)
        {
            var record = await _service.GetByIdAsync(id);
            if (record == null) return NotFound();
            return Ok(record);
        }

        //Lấy danh sách bệnh nhân của doctor
        [HttpGet("doctor/{doctorId}/patients")]
        [Authorize(Roles = "Staff,Manager,Doctor,Patient")]
        public async Task<IActionResult> GetDoctorPatients(int doctorId)
        {
            var patients = await _service.GetDoctorPatientsAsync(doctorId);
            return Ok(patients);
        }

        //Lấy medical records của 1 bệnh nhân cho doctor
        [HttpGet("doctor/{doctorId}/patient/{patientId}")]
        [Authorize(Roles = "Staff,Manager,Doctor,Patient")]
        public async Task<IActionResult> GetPatientRecordsForDoctor(int doctorId, int patientId)
        {
            var records = await _service.GetPatientRecordsForDoctorAsync(doctorId, patientId);
            return Ok(records);
        }

        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "Staff,Manager,Doctor,Patient")]
        public async Task<IActionResult> GetByDoctorId(int doctorId)
        {
            var records = await _service.GetByDoctorIdAsync(doctorId);
            return Ok(records);
        }

        [HttpGet("patient/{patientId}")]
        //[Authorize(Roles = "Staff,Manager,Doctor,Patient")]
        public async Task<IActionResult> GetByPatientId(int patientId)
        {
            var records = await _service.GetByPatientIdAsync(patientId);
            return Ok(records);
        }

        [HttpPost]
        //[Authorize(Roles = "Staff,Manager,Doctor,Patient")]
        public async Task<IActionResult> Create(CreateMedicalRecordByAppointmentDto dto)
        {
            var record = await _service.CreateByAppointmentIdAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = record.RecordId }, record);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Staff,Manager,Doctor,Patient")]
        public async Task<IActionResult> Update(int id, UpdateMedicalRecordDto dto)
        {
            var success = await _service.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff,Manager,Doctor,Patient")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        //Lấy thông tin chi tiết bao gồm Examination và ARV Protocol
        [HttpGet("{id}/detail")]
        [Authorize(Roles = "Staff,Manager,Doctor,Patient")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var record = await _service.GetDetailByIdAsync(id);
            if (record == null) return NotFound();
            return Ok(record);
        }
    }
}