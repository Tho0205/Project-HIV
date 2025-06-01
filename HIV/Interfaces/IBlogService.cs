// Services/IBlogService.cs
using HIV.DTOs;
using HIV.Interfaces;

public interface IBlogService
{
    Task<IEnumerable<BlogDto>> GetAllBlogsAsync();
    Task<BlogDto> GetBlogByIdAsync(int id);

    Task<BlogDto> CreateBlogAsync(BlogDto blogDto);
    Task<BlogDto> UpdateBlogAsync(int id, BlogDto blogDto);

    Task<BlogDto> ApproveBlogAsync(int id);
    Task<bool> DeleteBlogAsync(int id);

}
