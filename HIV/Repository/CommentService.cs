using AutoMapper;
using HIV.DTOs;
using HIV.Interfaces;
using HIV.Models;
using Microsoft.EntityFrameworkCore;

namespace HIV.Repository
{
    public class CommentService : ICommentService
    {

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CommentService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CommentDto> AddCommentAsync(CommentDto commentDto)
        {
            var comment = _mapper.Map<Comment>(commentDto);
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return _mapper.Map<CommentDto>(comment);
        }

        public async Task<List<CommentDto>> GetAllCommentsByBlogAsync(int blogId)
        {
            var comments = await _context.Comments
                .Where(c => c.BlogId == blogId && c.Status == "ACTIVE")
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return await Task.FromResult(_mapper.Map<List<CommentDto>>(comments));
        }
    }
}
