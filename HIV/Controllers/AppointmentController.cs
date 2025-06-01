using HIV.DTOs;
using HIV.DTOs.DTOAppointment;
using HIV.Interfaces;
using HIV.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.DataClassification;
using HIV.DTOs.DTOSchedule;
namespace HIV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        //private readonly ICommonOperation<Appointment> _repository;
        private readonly IAppointmentService _appService;
        public AppointmentController(IAppointmentService appointmentService)
        {
            //_repository = repository;
            _appService = appointmentService;
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
            await _appService.CreateAppointment(dto);
            return Ok("create success");
        }
    }   
}   
