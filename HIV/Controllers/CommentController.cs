using HIV.DTOs;
using HIV.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HIV.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("{blogId}")]
        public async Task<IActionResult> GetAllCommentsByBlog(int blogId)
        {
            var comments = await _commentService.GetAllCommentsByBlogAsync(blogId);
            if (comments == null || !comments.Any())
            {
                return NotFound("No comments found for this blog.");
            }
            return Ok(comments);
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddComment([FromBody] CommentDto commentDto)
        {
            if (commentDto == null)
            {
                return BadRequest("Comment data is null");
            }
            var createdComment = await _commentService.AddCommentAsync(commentDto);
            return CreatedAtAction(nameof(GetAllCommentsByBlog), new { blogId = createdComment.BlogId }, createdComment);

        }
    }
}
