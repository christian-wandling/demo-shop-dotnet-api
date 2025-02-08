#region

using AutoMapper;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Domain.User.Entities;

#endregion

namespace DemoShop.Application.Features.User.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserEntity, UserResponse>()
            .ForMember(dest => dest.Email, opt =>
                opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.Firstname, opt =>
                opt.MapFrom(src => src.PersonName.Firstname))
            .ForMember(dest => dest.Lastname, opt =>
                opt.MapFrom(src => src.PersonName.Lastname))
            .ForMember(dest => dest.Phone, opt =>
                opt.MapFrom(src => src.Phone.Value))
            .ForMember(dest => dest.Address, opt =>
                opt.MapFrom(src => src.Address));

        CreateMap<UserEntity, UserPhoneResponse>()
            .ForMember(dest => dest.Phone, opt =>
                opt.MapFrom(src => src.Phone.Value));

        CreateMap<AddressEntity, AddressResponse>()
            .ForMember(dest => dest.Street, opt =>
                opt.MapFrom(src => src.Street))
            .ForMember(dest => dest.Apartment, opt =>
                opt.MapFrom(src => src.Apartment))
            .ForMember(dest => dest.City, opt =>
                opt.MapFrom(src => src.City))
            .ForMember(dest => dest.Zip, opt =>
                opt.MapFrom(src => src.Zip))
            .ForMember(dest => dest.Region, opt =>
                opt.MapFrom(src => src.Region))
            .ForMember(dest => dest.Country, opt =>
                opt.MapFrom(src => src.Country));
    }
}
