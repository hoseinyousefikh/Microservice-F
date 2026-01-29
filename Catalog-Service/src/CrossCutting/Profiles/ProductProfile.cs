using AutoMapper;
using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._03_Endpoints.DTOs.Responses.Public;

namespace Catalog_Service.src.CrossCutting.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src =>
                    src.Images.OrderBy(i => i.Id).FirstOrDefault().PublicUrl))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        }
    }
}
