using HIV.DTOs;
using HIV.DTOs.DTOAppointment;
using HIV.Interfaces;
using HIV.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.DataClassification;
using HIV.DTOs.DTOSchedule;
using HIV.Repository;
namespace HIV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        //private readonly ICommonOperation<Appointment> _repository;
        private readonly IAppointmentService _appService;
        private readonly IDoctorMangamentPatient _doctorPatientService; // Thêm dòng này
        public AppointmentController(IAppointmentService appointmentService, IDoctorMangamentPatient doctorPatientService)
        {
            //_repository = repository;
            _appService = appointmentService;
            _doctorPatientService = doctorPatientService; // Thêm dòng này
        }
        //[HttpPost]
        //public async Task<IActionResult> CreateAppointment([FromBody] AppointmentDTO data, int doctor_id)
        //{

        //    return Ok();
        //}

        [HttpGet]

        public async Task<ActionResult<List<UserTableDTO>>> GetAllListDoctor()
        {
            var list = await _appService.GetAllListDoctor();
            return Ok(list);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<List<ScheduleSimpleDTO>>> GetScheduleDoctor([FromRoute]int id)
        {
            var schedules = await _appService.GetScheduleOfDoctor(id);
            return Ok(schedules);
        
        }
        [HttpGet("GetAllSchedule/{id:int}")]
        public async Task<ActionResult<List<ScheduleSimpleDTO>>> GetAllScheduleDoctor([FromRoute] int id)
        {
            var schedules = await _appService.GetAllScheduleOfDoctor(id);
            return Ok(schedules);

        }
        [HttpGet("patient/{id:int}")]

        public async Task<IActionResult> GetInforPatient(int id)
        {
            var inforPatient = await _appService.GetInforUser(id);

            return Ok(inforPatient);
        }

        [HttpPost]
        [Route("create")]
        public  async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDTO dto)
        {
            // Gọi auto transfer TRƯỚC khi tạo appointment
            await _doctorPatientService.AutoTransferPatientOnNewAppointment(dto.PatientId, dto.doctorId);
            //
            await _appService.CreateAppointment(dto);
            return Ok("create success");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var check = await _appService.CancelAppointment(id);
            if (!check) return NotFound("Không tìm thấy id");
            return Ok("delete success");
        }

        [HttpGet("GetAll")]

        public async Task<ActionResult<List<AppointmentDTO>>> GetAll()
        {
            var listAppoint = await _appService.GetAll();
            return Ok(listAppoint);
        }
        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<List<PatientOfDoctorDTO>>> GetPatientsOfDoctor(int doctorId)
        {
            try
            {
                var patients = await _appService.GetPatientsOfDoctor(doctorId);
                return Ok(patients);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
 
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateAppointmentStatus([FromBody] UpdateAppointmentStatusDTO dto)
        {
            try
            {
                var result = await _appService.UpdateAppointmentStatus(dto);
                if (result)
                {
                    return Ok(new { message = "Cập nhật status thành công", success = true });
                }
                return BadRequest(new { message = "Cập nhật status thất bại", success = false });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, success = false });
            }
        }

        [HttpPut("confirm/{appointmentId}")]
        public async Task<IActionResult> ConfirmAppointment(int appointmentId, [FromBody] string? note = null)
        {
            try
            {
                var result = await _appService.ConfirmAppointment(appointmentId, note);
                if (result)
                {
                    return Ok(new { message = "Xác nhận lịch hẹn thành công", success = true });
                }
                return BadRequest(new { message = "Xác nhận lịch hẹn thất bại", success = false });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, success = false });
            }
        }

        [HttpPut("complete/{appointmentId}")]
        public async Task<IActionResult> CompleteAppointment(int appointmentId, [FromBody] string? note = null)
        {
            try
            {
                var result = await _appService.CompleteAppointment(appointmentId, note);
                if (result)
                {
                    return Ok(new { message = "Hoàn thành lịch hẹn thành công", success = true });
                }
                return BadRequest(new { message = "Hoàn thành lịch hẹn thất bại", success = false });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, success = false });
            }
        }

        [HttpGet("{appointmentId}")]
        public async Task<IActionResult> GetAppointmentById(int appointmentId)
        {
            try
            {
                var appointment = await _appService.GetAppointmentById(appointmentId);
                return Ok(appointment);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message, success = false });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, success = false });
            }
        }

    }   
}   
