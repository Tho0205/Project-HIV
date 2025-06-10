using HIV.DTOs;
using HIV.Models;

namespace HIV.Interfaces
{
    public interface ICommentService
    {
        Task<List<CommentDto>> GetAllCommentsByBlogAsync(int blogId);
        Task<CommentDto> AddCommentAsync(CommentDto commentDto);
    }
}
