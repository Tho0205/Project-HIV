using HIV.DTOs.DTOARVs;

namespace HIV.Interfaces.ARVinterfaces
{
    public interface IARVProtocolDetailService
    {
        Task<IEnumerable<ARVProtocolDetailDto>> GetAllAsync();
        Task<ARVProtocolDetailDto?> GetByIdAsync(int id);
        Task<ARVProtocolDetailDto> CreateAsync(CreateARVProtocolDetailDto dto);
        Task<bool> UpdateAsync(int id, UpdateARVProtocolDetailDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
