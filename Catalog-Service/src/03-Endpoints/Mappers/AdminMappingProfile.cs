using AutoMapper;
using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Primitives;
using Catalog_Service.src._03_Endpoints.DTOs.Requests.Admin;
using Catalog_Service.src._03_Endpoints.DTOs.Responses.Admin;

namespace Catalog_Service.src._03_Endpoints.Mappers
{
    public class AdminMappingProfile : Profile
    {
        public AdminMappingProfile()
        {
            // Category mappings
            CreateMap<Category, AdminCategoryResponse>();
            CreateMap<CreateCategoryRequest, Category>();
            CreateMap<UpdateCategoryRequest, Category>();

            // Brand mappings
            CreateMap<Brand, AdminBrandResponse>();
            CreateMap<CreateBrandRequest, Brand>();
            CreateMap<UpdateBrandRequest, Brand>();

            // Product mappings
            CreateMap<Product, AdminProductResponse>()
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : 0))
                .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => src.Reviews.Count))
                .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes));

            CreateMap<ProductVariant, ProductVariantResponse>();
            CreateMap<ImageResource, ProductImageResponse>();
            CreateMap<ProductAttribute, ProductAttributeResponse>();

            // Value object mappings
            CreateMap<Dimensions, DimensionsResponse>();
            CreateMap<Weight, WeightResponse>();
        }
    }
}
