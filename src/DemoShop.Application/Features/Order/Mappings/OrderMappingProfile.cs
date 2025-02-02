using AutoMapper;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Product.Entities;
using DemoShop.Domain.ShoppingSession.Entities;

namespace DemoShop.Application.Features.Order.Mappings;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<OrderEntity, OrderResponse>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt =>
                opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Items, opt =>
                opt.MapFrom(src => src.OrderItems))
            .ForMember(dest => dest.Amount, opt =>
                opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Status, opt =>
                opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Created, opt =>
                opt.MapFrom(src => src.Audit.CreatedAt));

        CreateMap<OrderItemEntity, OrderItemResponse>()
            .ForMember(dest => dest.ProductId, opt =>
                opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.ProductName, opt =>
                opt.MapFrom(src => src.Product.ProductName))
            .ForMember(dest => dest.ProductThumbnail, opt =>
                opt.MapFrom(src => src.Product.ProductThumbnail))
            .ForMember(dest => dest.Quantity, opt =>
                opt.MapFrom(src => src.Quantity.Value))
            .ForMember(dest => dest.UnitPrice, opt =>
                opt.MapFrom(src => src.Price.Value))
            .ForMember(dest => dest.TotalPrice, opt =>
                opt.MapFrom(src => src.TotalPrice));

        CreateMap<IReadOnlyCollection<OrderEntity>, OrderListResponse>()
            .ForMember(dest => dest.Items, opt =>
                opt.MapFrom(src => src));

        CreateMap<CategoryEntity, string>();
        CreateMap<ImageEntity, ImageResponse>();
    }
}
