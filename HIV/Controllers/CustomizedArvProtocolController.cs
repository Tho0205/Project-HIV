using HIV.DTOs.DTOARVs;
using HIV.Interfaces.ARVinterfaces;
using HIV.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HIV.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomizedArvProtocolController : ControllerBase
    {
        private readonly ICustomizedArvProtocolService _service;

        public CustomizedArvProtocolController(ICustomizedArvProtocolService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomizedArvProtocolDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.CustomProtocolId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateCustomizedArvProtocolDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
