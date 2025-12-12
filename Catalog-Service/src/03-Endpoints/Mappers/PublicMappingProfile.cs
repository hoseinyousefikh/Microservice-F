using AutoMapper;
using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._01_Domain.Core.Primitives;
using Catalog_Service.src._03_Endpoints.DTOs.Responses.Public;
using static Catalog_Service.src._03_Endpoints.DTOs.Responses.Public.ProductVariantResponse;

namespace Catalog_Service.src._03_Endpoints.Mappers
{
    public class PublicMappingProfile : Profile
    {
        public PublicMappingProfile()
        {
            // Product mappings
            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : 0))
                .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => src.Reviews.Count))
                .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants.Where(v => v.IsActive)))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes.Where(a => !a.IsVariantSpecific)));

            CreateMap<ProductVariant, ProductVariantResponse>();
            CreateMap<ImageResource, ProductImageResponse>();
            CreateMap<ProductAttribute, ProductAttributeResponse>();

            // Category mappings
            CreateMap<Category, CategoryResponse>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count(p => p.Status == ProductStatus.Published)))
                .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories.Where(sc => sc.IsActive)));

            // Brand mappings
            CreateMap<Brand, BrandResponse>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count(p => p.Status == ProductStatus.Published)));

            // Review mappings
            CreateMap<ProductReview, ProductReviewResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserId)); // In real app, you might get user name from user service

            // Value object mappings
            CreateMap<Dimensions, DimensionsResponse>();
            CreateMap<Weight, WeightResponse>();
        }
    }
}
