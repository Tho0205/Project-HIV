using HIV.DTOs.DTOARVs;

namespace HIV.Interfaces.ARVinterfaces
{
    public interface ICustomizedArvProtocolDetailService
    {
        Task<IEnumerable<CustomizedArvProtocolDetailDto>> GetAllAsync();
        Task<CustomizedArvProtocolDetailDto?> GetByIdAsync(int id);
        Task<CustomizedArvProtocolDetailDto> CreateAsync(CreateCustomizedArvProtocolDetailDto dto);
        Task<bool> UpdateAsync(int id, UpdateCustomizedArvProtocolDetailDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
