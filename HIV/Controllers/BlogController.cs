
using HIV.DTOs;
using HIV.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HIV.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class BlogController : ControllerBase
    {
        private readonly IBlogRepository _blogRepo;
        private readonly IWebHostEnvironment _env;
        public BlogController(IBlogRepository blogRepository, IWebHostEnvironment env)
        {
            _blogRepo = blogRepository;
            _env = env;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllBlogs()
        {
            var blogs = await _blogRepo.GetAllBlogsAsync();
            return Ok(blogs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            var blog = await _blogRepo.GetBlogByIdAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return Ok(blog);

        }

        [HttpPost("create")]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<IActionResult> CreateBlog([FromBody] BlogDto blogDto)
        {
            if (blogDto == null)
            {
                return BadRequest("Blog data is null");
            }
            var createdBlog = await _blogRepo.CreateBlogAsync(blogDto);
            return CreatedAtAction(nameof(GetBlogById), new { id = createdBlog.BlogId }, createdBlog);
        }

        [HttpPut("update")]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<IActionResult> UpdateBlog(int id, [FromBody] BlogDto blogDto)
        {
            if (blogDto == null )
            {
                return BadRequest("Blog data is invalid");
            }
            var updatedBlog = await _blogRepo.UpdateBlogAsync(id, blogDto);
            if (updatedBlog == null)
            {
                return NotFound();
            }
            return Ok(updatedBlog);
        }

        [HttpPut("approve/{id}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> ApproveBlog(int id)
        {
            var approvedBlog = await _blogRepo.ApproveBlogAsync(id);
            if (approvedBlog == null)
            {
                return NotFound();
            }
            return Ok(approvedBlog);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Doctor,Patient,Staff,Manager")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var isDeleted = await _blogRepo.DeleteBlogAsync(id);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                // Kiểm tra loại file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest("Invalid file type. Only images are allowed");

                // Giới hạn kích thước 
                if (file.Length > 5 * 1024 * 1024)
                    return BadRequest("File size exceeds 5MB limit");

                // Tạo thư mục nếu chưa tồn tại
                var uploadPath = Path.Combine(_env.ContentRootPath, "Uploads", "Blogs");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // Tạo tên file an toàn
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadPath, fileName);

                // Lưu file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Trả về URL truy cập
                var imageUrl = $"/Uploads/Blogs/{fileName}";
                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
