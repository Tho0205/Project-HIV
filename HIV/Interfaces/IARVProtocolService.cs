using HIV.DTOs.DTOARVs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HIV.Interfaces.ARVinterfaces
{
    public interface IARVProtocolService
    {
        Task<IEnumerable<ARVProtocolDto>> GetAllAsync();
        Task<ServiceResult<ARVProtocolDto>> GetFullProtocolByIdAsync(int id);
        Task<ServiceResult<ARVProtocolDto>> CreateWithDetailsAsync(CreateARVProtocolWithDetailsDto dto);
        Task<ServiceResult<ARVProtocolDto>> UpdateProtocolWithDetailsAsync(int protocolId, UpdateARVProtocolWithDetailsDto dto);
        Task<bool> DeleteAsync(int id);
        Task<ServiceResult<ARVProDetailDto>> AddSingleARVToProtocolAsync(int protocolId, AddARVToProtocolDto dto);
        Task<ServiceResult<ARVProDetailDto>> UpdateProtocolDetailAsync(int protocolId, int detailId, UpdateARVProtocolDetailDto dto);
        Task<ServiceResult<bool>> HardRemoveDetailFromProtocolAsync(int protocolId, int detailId);
        Task<ServiceResult<List<ARVProDetailDto>>> GetProtocolActiveDetailsAsync(int protocolId);
    }
}