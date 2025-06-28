using Authentication.Contracts.DTOs;
using Authentication.DAL.Models;
using AutoMapper;

namespace Authentication.Services;

public class AuthenticationProfile : Profile 
{
    public AuthenticationProfile()
    {
        CreateMap<RegisterRequest, User>()
            .ForMember(dest => dest.id, opt => opt.MapFrom(_ => Guid.NewGuid())).ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
    }
}