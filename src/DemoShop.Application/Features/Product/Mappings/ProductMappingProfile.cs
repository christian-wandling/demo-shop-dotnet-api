using AutoMapper;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.Domain.Product.Entities;

namespace DemoShop.Application.Features.Product.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<ProductEntity, ProductResponse>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt =>
                opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt =>
                opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Price, opt =>
                opt.MapFrom(src => src.Price.Value))
            .ForMember(dest => dest.Categories, opt =>
                opt.MapFrom(src => src.Categories))
            .ForMember(dest => dest.Images, opt =>
                opt.MapFrom(src => src.Images))
            .ForMember(dest => dest.Thumbnail, opt =>
                opt.MapFrom(src => src.Images.FirstOrDefault()));

        CreateMap<List<ProductEntity>, ProductListResponse>()
            .ForMember(dest => dest.Products, opt =>
                opt.MapFrom(src => src));

        CreateMap<CategoryEntity, string>();
        CreateMap<ImageEntity, ImageResponse>();
    }
}
