using HIV.DTOs;
using HIV.Interfaces;


namespace HIV.Interfaces
{
    public interface IEducationalResourcesService
    {
        Task<IEnumerable<EducationalResourcesDto>> GetAllAsync();
    }
}
