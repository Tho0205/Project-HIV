using AutoMapper;
using HIV.Models;
using HIV.Interfaces;



namespace HIV.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add your mapping configurations here
            // For example:
            // CreateMap<SourceType, DestinationType>();
            CreateMap<FacilityInfo, FacilityInfoDto>().ReverseMap();
            CreateMap<Blog, BlogDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author != null ? src.Author.FullName : string.Empty));
            CreateMap<BlogDto, Blog>()
                .ForMember(dest => dest.Author, opt => opt.Ignore());
            CreateMap<EducationalResource, EducationalResourcesDto>()
                //.ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.FullName : string.Empty))
                .ReverseMap();

        }
    }

}
