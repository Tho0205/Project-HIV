using AutoMapper;

using HIV.DTOs;
using HIV.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HIV.Repository
{
    public class EducationalResourcesService : IEducationalResourcesService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public EducationalResourcesService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EducationalResourcesDto>> GetAllAsync()
        {
            var educationalResources = await _context.EducationalResources.ToListAsync();
            return _mapper.Map<IEnumerable<EducationalResourcesDto>>(educationalResources);
        }
    }
}
