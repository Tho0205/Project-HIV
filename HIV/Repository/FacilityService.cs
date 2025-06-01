using AutoMapper;
using HIV.Data;
using HIV.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HIV.Repository
{
    public class FacilityService : IFacilityService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public FacilityService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FacilityInfoDto>> GetAllFacilitiesAsync()
        {
            var facilities = await _context.FacilityInfos.ToListAsync();
            return _mapper.Map<IEnumerable<FacilityInfoDto>>(facilities);
        }
    }
}
