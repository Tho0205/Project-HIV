using HIV.DTOs.DTOARVs;

namespace HIV.Interfaces.ARVinterfaces
{
    public interface IArvService
    {
        Task<IEnumerable<ArvDto>> GetAllAsync();
        Task<ArvDto?> GetByIdAsync(int id);
        Task<ArvDto> CreateAsync(CreateArvDto dto);
        Task<bool> UpdateAsync(int id, UpdateArvDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
