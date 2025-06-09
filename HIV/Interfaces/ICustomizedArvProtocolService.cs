using HIV.DTOs.DTOARVs;

namespace HIV.Interfaces.ARVinterfaces
{
    public interface ICustomizedArvProtocolService
    {
        Task<IEnumerable<CustomizedArvProtocolDto>> GetAllAsync();
        Task<CustomizedArvProtocolDto?> GetByIdAsync(int id);
        Task<CustomizedArvProtocolDto> CreateAsync(CreateCustomizedArvProtocolDto dto);
        Task<bool> UpdateAsync(int id, UpdateCustomizedArvProtocolDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
