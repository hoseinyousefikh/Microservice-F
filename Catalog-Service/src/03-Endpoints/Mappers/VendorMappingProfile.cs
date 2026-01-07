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
            // Product mappings (اصلاح شده)
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
                    Weight.Create(src.Weight, "kg")))
                // CreatedByUserId باید در کنترلر قبل از فراخوانی سرویس تنظیم شود.
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateProductRequest, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Slug, opt => opt.Ignore())
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => Money.Create(src.Price, "USD")))
                .ForMember(dest => dest.OriginalPrice, opt => opt.MapFrom(src => src.OriginalPrice.HasValue ?
                    Money.Create(src.OriginalPrice.Value, "USD") : null))
                .ForMember(dest => dest.Dimensions, opt => opt.MapFrom(src =>
                    Dimensions.Create(src.Dimensions.Length, src.Dimensions.Width, src.Dimensions.Height, "cm")))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src =>
                    Weight.Create(src.Weight, "kg")))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Product variant mappings (اصلاح شده)
            CreateMap<ProductVariant, VendorProductVariantResponse>();

            CreateMap<CreateProductVariantRequest, ProductVariant>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => Money.Create(src.Price, "USD")))
                .ForMember(dest => dest.OriginalPrice, opt => opt.MapFrom(src => src.OriginalPrice.HasValue ?
                    Money.Create(src.OriginalPrice.Value, "USD") : null))
                .ForMember(dest => dest.Dimensions, opt => opt.MapFrom(src =>
                    Dimensions.Create(src.Dimensions.Length, src.Dimensions.Width, src.Dimensions.Height, "cm")))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src =>
                    Weight.Create(src.Weight, "kg")))
                // CreatedByUserId برای ProductVariant تنظیم نمی‌شود، چون مالکیت آن از طریق Product مشخص است.
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateProductVariantRequest, ProductVariant>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => Money.Create(src.Price, "USD")))
                .ForMember(dest => dest.OriginalPrice, opt => opt.MapFrom(src => src.OriginalPrice.HasValue ?
                    Money.Create(src.OriginalPrice.Value, "USD") : null))
                .ForMember(dest => dest.Dimensions, opt => opt.MapFrom(src =>
                    Dimensions.Create(src.Dimensions.Length, src.Dimensions.Width, src.Dimensions.Height, "cm")))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src =>
                    Weight.Create(src.Weight, "kg")))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Other mappings (بدون تغییر)
            CreateMap<ImageResource, VendorProductImageResponse>();
            CreateMap<ProductAttribute, VendorProductAttributeResponse>();

            // Value object mappings
            CreateMap<Dimensions, VendorDimensionsResponse>();
            CreateMap<Weight, VendorWeightResponse>();
        }
    }
}