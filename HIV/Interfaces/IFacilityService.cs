using HIV.DTOs;
using HIV.Interfaces;


namespace HIV.Interfaces
{
    public interface IFacilityService
    {
        Task<IEnumerable<FacilityInfoDto>> GetAllFacilitiesAsync();
    }
}
