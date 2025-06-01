
using HIV.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HIV_Treatment_System_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;
        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllBlogs()
        {
            var blogs = await _blogService.GetAllBlogsAsync();
            return Ok(blogs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            var blog = await _blogService.GetBlogByIdAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return Ok(blog);

        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBlog([FromBody] BlogDto blogDto)
        {
            if (blogDto == null)
            {
                return BadRequest("Blog data is null");
            }
            var createdBlog = await _blogService.CreateBlogAsync(blogDto);
            return CreatedAtAction(nameof(GetBlogById), new { id = createdBlog.BlogId }, createdBlog);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateBlog(int id, [FromBody] BlogDto blogDto)
        {
            if (blogDto == null )
            {
                return BadRequest("Blog data is invalid");
            }
            var updatedBlog = await _blogService.UpdateBlogAsync(id, blogDto);
            if (updatedBlog == null)
            {
                return NotFound();
            }
            return Ok(updatedBlog);
        }

        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveBlog(int id)
        {
            var approvedBlog = await _blogService.ApproveBlogAsync(id);
            if (approvedBlog == null)
            {
                return NotFound();
            }
            return Ok(approvedBlog);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var isDeleted = await _blogService.DeleteBlogAsync(id);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
