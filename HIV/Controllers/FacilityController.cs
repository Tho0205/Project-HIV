
using HIV.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HIV.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacilityController : ControllerBase
    {
        private readonly IFacilityService _facilityService;
        public FacilityController(IFacilityService facilityService)
        {
            _facilityService = facilityService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllFacilities()
        {
            var facilities = await _facilityService.GetAllFacilitiesAsync();
            return Ok(facilities);
        }
    }
}
