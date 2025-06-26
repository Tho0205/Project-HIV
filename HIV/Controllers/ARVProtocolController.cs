using HIV.DTOs.DTOARVs;
using HIV.Interfaces.ARVinterfaces;
using Microsoft.AspNetCore.Mvc;

namespace HIV.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ARVProtocolController : ControllerBase
    {
        private readonly IARVProtocolService _service;

        public ARVProtocolController(IARVProtocolService service)
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
        public async Task<IActionResult> Create(CreateARVProtocolDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ProtocolId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateARVProtocolDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (!updated) return NotFound();

            var item = await _service.GetByIdAsync(id); // ✅ Trả lại bản ghi mới
            return Ok(item);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();

            return Ok(new { success = true, message = "ARV Protocol đã được xoá (mềm)." });
        }

        [HttpGet("{id}/arv-details")]
        public async Task<IActionResult> GetARVDetails(int id)
        {
            var details = await _service.GetARVDetailsByProtocolIdAsync(id);

            if (details == null || !details.Any())
            {
                return NotFound(new { message = "Không tìm thấy chi tiết ARV tương ứng." });
            }

            return Ok(details);
        }
        //-----------------------------------------------------------------------------------------
        [HttpPost("create-with-details")]
        public async Task<IActionResult> CreateWithDetails([FromBody] CreateARVProtocolWithDetailsDto dto)
        {
            var result = await _service.CreateWithDetailsAsync(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return CreatedAtAction(
                nameof(GetFullProtocol),
                new { id = result.Data.ProtocolId },
                result.Data);
        }

        [HttpPost("{id}/add-details")]
        public async Task<IActionResult> AddDetails(int id, [FromBody] List<CreateARVProtocolDetailDto> details)
        {
            var result = await _service.AddDetailsToProtocolAsync(id, details);

            if (!result.IsSuccess)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Data);
        }

        [HttpPut("{protocolId}/details/{detailId}")]
        public async Task<IActionResult> UpdateDetail(
            int protocolId,
            int detailId,
            [FromBody] CreateARVProtocolDetailDto dto)
        {
            var result = await _service.UpdateProtocolDetailAsync(protocolId, detailId, dto);

            if (!result.IsSuccess)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Data);
        }

        [HttpDelete("{protocolId}/details/{detailId}")]
        public async Task<IActionResult> RemoveDetail(int protocolId, int detailId)
        {
            var result = await _service.RemoveDetailFromProtocolAsync(protocolId, detailId);

            if (!result.IsSuccess)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(new { success = true, message = "Đã xoá ARV khỏi protocol" });
        }

        [HttpGet("{id}/full")]
        public async Task<IActionResult> GetFullProtocol(int id)
        {
            var result = await _service.GetFullProtocolByIdAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(new { errors = result.Errors });
            }

            return Ok(result.Data);
        }
    }
}
    
