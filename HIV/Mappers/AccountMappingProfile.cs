using AutoMapper;
using HIV.Models;
using HIV.DTOs;

namespace HIV.Mapping
{
    public class AccountMappingProfile : Profile
    {
        public AccountMappingProfile()
        {
            // Account mappings
            CreateMap<Account, AccountDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

            CreateMap<Account, AccountInfoDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

            CreateMap<CreateAccountDto, Account>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.AccountId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.MapFrom(src =>
                    src.Role != null ? new User { Role = src.Role } : null));

            CreateMap<UpdateAccountDto, Account>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.MapFrom(src =>
                    src.Role != null ? new User { Role = src.Role } : null));

            // User mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Account.Email));

            CreateMap<User, UserInfoDto>();
        }
    }
}