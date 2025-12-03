using AutoMapper;
using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Primitives;
using Catalog_Service.src._03_Endpoints.DTOs.Requests.Vendor;
using Catalog_Service.src._03_Endpoints.DTOs.Responses.Vendor;

namespace Catalog_Service.src._03_Endpoints.Mappers
{
    public class VendorMappingProfile : Profile
    {
        public VendorMappingProfile()
        {
            // Product mappings
            CreateMap<Product, VendorProductResponse>()
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : 0))
                .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => src.Reviews.Count))
                .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes));

            CreateMap<CreateProductRequest, Product>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => Money.Create(src.Price, "USD")))
                .ForMember(dest => dest.OriginalPrice, opt => opt.MapFrom(src => src.OriginalPrice.HasValue ?
                    Money.Create(src.OriginalPrice.Value, "USD") : null))
                .ForMember(dest => dest.Dimensions, opt => opt.MapFrom(src =>
                    Dimensions.Create(src.Dimensions.Length, src.Dimensions.Width, src.Dimensions.Height, "cm")))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src =>
                    Weight.Create(src.Weight, "kg")));

            CreateMap<UpdateProductRequest, Product>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => Money.Create(src.Price, "USD")))
                .ForMember(dest => dest.OriginalPrice, opt => opt.MapFrom(src => src.OriginalPrice.HasValue ?
                    Money.Create(src.OriginalPrice.Value, "USD") : null))
                .ForMember(dest => dest.Dimensions, opt => opt.MapFrom(src =>
                    Dimensions.Create(src.Dimensions.Length, src.Dimensions.Width, src.Dimensions.Height, "cm")))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src =>
                    Weight.Create(src.Weight, "kg")));

            // Product variant mappings
            CreateMap<ProductVariant, VendorProductVariantResponse>();

            CreateMap<CreateProductVariantRequest, ProductVariant>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => Money.Create(src.Price, "USD")))
                .ForMember(dest => dest.OriginalPrice, opt => opt.MapFrom(src => src.OriginalPrice.HasValue ?
                    Money.Create(src.OriginalPrice.Value, "USD") : null))
                .ForMember(dest => dest.Dimensions, opt => opt.MapFrom(src =>
                    Dimensions.Create(src.Dimensions.Length, src.Dimensions.Width, src.Dimensions.Height, "cm")))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src =>
                    Weight.Create(src.Weight, "kg")));

            CreateMap<UpdateProductVariantRequest, ProductVariant>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => Money.Create(src.Price, "USD")))
                .ForMember(dest => dest.OriginalPrice, opt => opt.MapFrom(src => src.OriginalPrice.HasValue ?
                    Money.Create(src.OriginalPrice.Value, "USD") : null))
                .ForMember(dest => dest.Dimensions, opt => opt.MapFrom(src =>
                    Dimensions.Create(src.Dimensions.Length, src.Dimensions.Width, src.Dimensions.Height, "cm")))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src =>
                    Weight.Create(src.Weight, "kg")));

            // Other mappings
            CreateMap<ImageResource, VendorProductImageResponse>();
            CreateMap<ProductAttribute, VendorProductAttributeResponse>();

            // Value object mappings
            CreateMap<Dimensions, VendorDimensionsResponse>();
            CreateMap<Weight, VendorWeightResponse>();
        }
    }
}