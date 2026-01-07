using AutoMapper;
using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Primitives;
using Catalog_Service.src._03_Endpoints.DTOs.Requests.Admin;
using Catalog_Service.src._03_Endpoints.DTOs.Requests.Vendor;
using Catalog_Service.src._03_Endpoints.DTOs.Responses.Admin;

namespace Catalog_Service.src._03_Endpoints.Mappers
{
    public class AdminMappingProfile : Profile
    {
        public AdminMappingProfile()
        {
            // --- Value Object Mappings ---
            CreateMap<decimal, Money>()
                .ConstructUsing(src => Money.Create(src, "IRR"));

            CreateMap<decimal?, Money?>()
                .ConstructUsing(src => src.HasValue ? Money.Create(src.Value, "IRR") : null);

            CreateMap<Money, decimal>()
                .ConvertUsing(src => src.Amount);

            CreateMap<Money?, decimal?>()
                .ConstructUsing(src => src != null ? src.Amount : (decimal?)null);

            CreateMap<DimensionsRequest, Dimensions>()
                .ConstructUsing(src => Dimensions.Create(src.Length, src.Width, src.Height, "cm"));

            CreateMap<Dimensions, DimensionsResponse>();

            // فرض: کلاس Weight دارای پراپرتی های Value و Unit است
            CreateMap<Weight, WeightResponse>()
                .ConstructUsing(src => new WeightResponse
                {
                    Value = src.Value,
                    Unit = src.Unit
                });

            // --- Request to Entity Mappings (اصلاح شده) ---
            CreateMap<CreateProductRequest, Product>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => Money.Create(src.Price, "IRR")))
                .ForMember(dest => dest.OriginalPrice, opt => opt.MapFrom(src =>
                    src.OriginalPrice.HasValue ? Money.Create(src.OriginalPrice.Value, "IRR") : null))
                .ForMember(dest => dest.Dimensions, opt => opt.MapFrom(src =>
                    Dimensions.Create(src.Dimensions.Length, src.Dimensions.Width, src.Dimensions.Height, "cm")))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => Weight.Create(src.Weight, "kg")))
                // CreatedByUserId باید در کنترلر قبل از فراخوانی سرویس تنظیم شود.
                // این مپینگ دیگر به Constructor نیاز ندارد.
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<CreateCategoryRequest, Category>()
                // CreatedByUserId باید در کنترلر قبل از فراخوانی سرویس تنظیم شود.
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateCategoryRequest, Category>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CreateBrandRequest, Brand>()
                // CreatedByUserId باید در کنترلر قبل از فراخوانی سرویس تنظیم شود.
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateBrandRequest, Brand>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // --- Entity to Response Mappings (بدون تغییر) ---
            CreateMap<Product, AdminProductResponse>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
                .ForMember(dest => dest.OriginalPrice, opt => opt.MapFrom(src =>
                    src.OriginalPrice != null ? src.OriginalPrice.Amount : (decimal?)null))
                .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug.Value))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src =>
                    src.Brand != null ? src.Brand.Name : string.Empty))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src =>
                    src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src =>
                    src.Images.Any(i => i.IsPrimary) ? src.Images.First(i => i.IsPrimary).PublicUrl : null))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                    src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : (double)0))
                .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => src.Reviews.Count));

            CreateMap<Category, AdminCategoryResponse>();
            CreateMap<Brand, AdminBrandResponse>();

            CreateMap<ProductVariant, ProductVariantResponse>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
                .ForMember(dest => dest.OriginalPrice, opt => opt.MapFrom(src =>
                    src.OriginalPrice != null ? src.OriginalPrice.Amount : (decimal?)null));

            CreateMap<ImageResource, ProductImageResponse>();
            CreateMap<ProductAttribute, ProductAttributeResponse>();

            CreateMap<ProductReview, AdminProductReviewResponse>()
               .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
               .ForMember(dest => dest.HasImages, opt => opt.MapFrom(src => src.Images.Any()))
               .ForMember(dest => dest.ReplyCount, opt => opt.MapFrom(src => src.Replies.Count));
        }
    }
}