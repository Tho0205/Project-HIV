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
        Task<ServiceResult<ARVProtocolDto>> CreateWithDetailsAsync(CreateARVProtocolWithDetailsDto dto);

        /// <summary>
        /// Thêm danh sách ARV vào protocol có sẵn
        /// </summary>
        Task<ServiceResult<ARVProtocolDto>> AddDetailsToProtocolAsync(int protocolId, List<CreateARVProtocolDetailDto> details);

        /// <summary>
        /// Cập nhật thông tin chi tiết của 1 ARV trong protocol
        /// </summary>
        Task<ServiceResult<ARVProDetailDto>> UpdateProtocolDetailAsync(int protocolId, int detailId, CreateARVProtocolDetailDto dto);

        /// <summary>
        /// Xóa 1 ARV chi tiết khỏi protocol (chuyển status sang INACTIVE)
        /// </summary>
        Task<ServiceResult<bool>> RemoveDetailFromProtocolAsync(int protocolId, int detailId);
        Task<ServiceResult<ARVProtocolDto>> GetFullProtocolByIdAsync(int id);
    }
}
