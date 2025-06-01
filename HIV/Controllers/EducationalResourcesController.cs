
using HIV.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace HIV_Treatment_System_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EducationalResourcesController : ControllerBase
    {
        private readonly IEducationalResourcesService _educationalResourcesService;

        public EducationalResourcesController(IEducationalResourcesService educationalResourcesService)
        {
            _educationalResourcesService = educationalResourcesService;
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetAllEducationalResources()
        {
            var resources = await _educationalResourcesService.GetAllAsync();
            return Ok(resources);
        }

    }
}
