using AutoMapper;
using HIV.Models;
using HIV.Interfaces;
using Microsoft.EntityFrameworkCore;
using HIV.DTOs;

namespace HIV.Repository
{
    public class BlogService : IBlogService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BlogService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BlogDto>> GetAllBlogsAsync()
        {
            var blogs = await _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Comments)
                .ToListAsync();

            return _mapper.Map<IEnumerable<BlogDto>>(blogs);
        }

        public async Task<BlogDto> GetBlogByIdAsync(int id)
        {
            var blog = await _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Comments)
                .FirstOrDefaultAsync(b => b.BlogId == id);

            return blog == null ? null : _mapper.Map<BlogDto>(blog);
        }

        public async Task<BlogDto> CreateBlogAsync(BlogDto blogDto)
        {
            var blog = _mapper.Map<Blog>(blogDto);
            blog.AuthorId = blogDto.AuthorId;
            blog.IsApproved = false;
            blog.CreatedAt = DateTime.Now;

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            return _mapper.Map<BlogDto>(blog);
        }

        public async Task<BlogDto> UpdateBlogAsync(int id, BlogDto blogDto)
        {
            var existingBlog = await _context.Blogs.FindAsync(id);
            if (existingBlog == null)
                return null;

            existingBlog.Title = blogDto.Title;
            existingBlog.Content = blogDto.Content;
            existingBlog.IsApproved = false;
            existingBlog.CreatedAt = DateTime.Now;

            _context.Blogs.Update(existingBlog);
            await _context.SaveChangesAsync();

            return _mapper.Map<BlogDto>(existingBlog);
        }

        public async Task<BlogDto> ApproveBlogAsync(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
                return null;

            blog.IsApproved = true;
            _context.Blogs.Update(blog);
            await _context.SaveChangesAsync();

            return _mapper.Map<BlogDto>(blog);
        }

        public async Task<bool> DeleteBlogAsync(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
                return false;

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
