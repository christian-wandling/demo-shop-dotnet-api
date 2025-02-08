#region

using AutoMapper;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Domain.ShoppingSession.Entities;

#endregion

namespace DemoShop.Application.Features.ShoppingSession.Mappings;

public class ShoppingSessionMappingProfile : Profile
{
    public ShoppingSessionMappingProfile()
    {
        CreateMap<ShoppingSessionEntity, ShoppingSessionResponse>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt =>
                opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Items, opt =>
                opt.MapFrom(src => src.CartItems));

        CreateMap<CartItemEntity, CartItemResponse>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ProductId, opt =>
                opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.ProductName, opt =>
                opt.MapFrom(src => src.Product!.Name))
            .ForMember(dest => dest.ProductThumbnail, opt =>
                opt.MapFrom(src => src.Product!.Thumbnail))
            .ForMember(dest => dest.Quantity, opt =>
                opt.MapFrom(src => src.Quantity.Value))
            .ForMember(dest => dest.UnitPrice, opt =>
                opt.MapFrom(src => src.Product!.Price.Value))
            .ForMember(dest => dest.TotalPrice, opt =>
                opt.MapFrom(src => src.TotalPrice));

        CreateMap<CartItemEntity, UpdateCartItemQuantityResponse>()
            .ForMember(dest => dest.Quantity, opt =>
                opt.MapFrom(src => src.Quantity.Value));
    }
}
