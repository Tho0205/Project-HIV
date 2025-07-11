using HIV.DTOs.DTOARVs;
using HIV.Interfaces.ARVinterfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var result = await _service.GetFullProtocolByIdAsync(id);
            if (!result.IsSuccess) return NotFound(result.Errors);
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateARVProtocolDto dto)
        {
            var result = await _service.CreateWithDetailsAsync(new CreateARVProtocolWithDetailsDto
            {
                Name = dto.Name,
                Description = dto.Description,
                Status = dto.Status,
                Details = new List<CreateARVProtocolDetailDto>()
            });

            if (!result.IsSuccess) return BadRequest(result.Errors);

            return CreatedAtAction(nameof(GetById), new { id = result.Data.ProtocolId }, result.Data);
        }

        [HttpPost("create-details")]
        public async Task<IActionResult> CreateWithDetails([FromBody] CreateARVProtocolWithDetailsDto dto)
        {
            var result = await _service.CreateWithDetailsAsync(dto);
            if (!result.IsSuccess) return BadRequest(result.Errors);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Data.ProtocolId },
                result.Data
            );
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateARVProtocolDto dto)
        {




            var currentProtocol = await _service.GetFullProtocolByIdAsync(id);
            if (!currentProtocol.IsSuccess) return NotFound(currentProtocol.Errors);



            var result = await _service.UpdateProtocolWithDetailsAsync(id, new UpdateARVProtocolWithDetailsDto
            {
                Name = dto.Name,
                Description = dto.Description,
                Status = dto.Status,
                Details = currentProtocol.Data.Details.Select(d => new UpdateARVProtocolDetailDto
                {
                    DetailId = d.DetailId,
                    ArvId = d.ArvId,
                    Dosage = d.Dosage,
                    UsageInstruction = d.UsageInstruction,
                    Status = d.Status
                }).ToList()
            });

            if (!result.IsSuccess) return BadRequest(result.Errors);


            return Ok(result.Data);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return Ok(new { success = true, message = "Protocol đã được đánh dấu xóa" });
        }

        [HttpGet("{id}/arv-details")]
        public async Task<IActionResult> GetARVDetails(int id)
        {
            var result = await _service.GetProtocolActiveDetailsAsync(id);
            if (!result.IsSuccess) return NotFound(result.Errors);
            return Ok(result.Data);
        }

        [HttpPost("{protocolId}/add-arv")]
        public async Task<IActionResult> AddARVToProtocol(int protocolId, [FromBody] AddARVToProtocolDto dto)
        {
            var result = await _service.AddSingleARVToProtocolAsync(protocolId, dto);
            if (!result.IsSuccess) return BadRequest(result.Errors);
            return Ok(result.Data);
        }

        [HttpPut("{protocolId}/details/{detailId}")]
        public async Task<IActionResult> UpdateDetail(
            int protocolId,
            int detailId,
            [FromBody] UpdateARVProtocolDetailDto dto)
        {
            if (protocolId <= 0 || detailId <= 0)
                return BadRequest(new { errors = new[] { "ID protocol hoặc detail không hợp lệ" } });

            if (dto.DetailId != detailId)
                return BadRequest(new { errors = new[] { "ID chi tiết không khớp" } });

            var result = await _service.UpdateProtocolDetailAsync(protocolId, detailId, dto);
            if (!result.IsSuccess) return BadRequest(result.Errors);


            return Ok(result.Data);
        }

        [HttpDelete("{protocolId}/details/{detailId}")]
        public async Task<IActionResult> RemoveDetail(int protocolId, int detailId)
        {
            var result = await _service.HardRemoveDetailFromProtocolAsync(protocolId, detailId);
            if (!result.IsSuccess) return BadRequest(result.Errors);
            return Ok(new { success = true, message = "Đã xóa ARV khỏi protocol" });
        }
    }
}