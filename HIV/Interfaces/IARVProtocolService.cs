using HIV.DTOs.DTOARVs;
using HIV.Models;

namespace HIV.Interfaces.ARVinterfaces
{
    public interface IARVProtocolService
    {
        Task<IEnumerable<ARVProtocolDto>> GetAllAsync();
        Task<ARVProtocolDto?> GetByIdAsync(int id);
        Task<ARVProtocolDto> CreateAsync(CreateARVProtocolDto dto);
        Task<bool> UpdateAsync(int id, UpdateARVProtocolDto dto);
        Task<bool> DeleteAsync(int id);
       Task<IEnumerable<ARVProDetailDto>> GetARVDetailsByProtocolIdAsync(int protocolId);
    }
}
