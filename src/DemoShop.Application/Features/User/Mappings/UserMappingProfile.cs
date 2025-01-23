using AutoMapper;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Domain.User.Entities;

namespace DemoShop.Application.Features.User.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserEntity, UserResponse>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt =>
                opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Firstname, opt =>
                opt.MapFrom(src => src.PersonName.Firstname))
            .ForMember(dest => dest.Lastname, opt =>
                opt.MapFrom(src => src.PersonName.Lastname))
            .ForMember(dest => dest.Phone, opt =>
                opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.Address, opt =>
                opt.MapFrom(src => src.Address));

        CreateMap<AddressEntity, AddressResponse>();
    }
}
