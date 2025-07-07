using HIV.DTOs;
using HIV.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace HIV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Staff,Admin")]
    public class DoctorInfoController : ControllerBase
    {
        private readonly IDoctorInfoService _service;

        public DoctorInfoController(IDoctorInfoService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Staff,Admin")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Create(CreateDoctorInfoDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.DoctorId }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Update(int id, UpdateDoctorInfoDto dto)
        {
            var success = await _service.UpdateAsync(id, dto);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpGet("active")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> GetActiveDoctors()
        {
            var data = await _service.GetAllAsync();
            var activeDoctors = data.Where(d => d.Status == "ACTIVE");
            return Ok(activeDoctors);
        }
    }
}
