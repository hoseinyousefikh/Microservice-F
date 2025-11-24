//using AutoMapper;
//using Identity_Service.Domain.Entities;
//using Identity_Service.Domain.Entities.ValueObjects;
//using Identity_Service.Domain.Enums;
//using Identity_Service.Infrastructure.Persistence.Entities;
//using Identity_Service.Presentation.Dtos.Responses.Roles;
//using Identity_Service.Presentation.Dtos.Responses.UserManagement;
//// اضافه کردن aliases برای جلوگیری از تداخل
//using InfraUser = Identity_Service.Infrastructure.Persistence.Entities.User;
//using InfraRole = Identity_Service.Infrastructure.Persistence.Entities.Role;

//namespace Identity_Service.Application.Common.Mappings
//{
//    public class MappingProfile : Profile
//    {
//        public MappingProfile()
//        {
//            // User mappings: Domain to Infrastructure
//            CreateMap<Domain.Entities.User, InfraUser>()
//                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
//                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
//                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
//                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber != null ? src.PhoneNumber.Value : null))
//                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
//                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
//                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
//                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
//                .ForMember(dest => dest.LastModifiedAt, opt => opt.MapFrom(src => src.UpdatedAt))
//                .ForMember(dest => dest.LastLoginAt, opt => opt.MapFrom(src => src.LastLoginAt))
//                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => src.EmailConfirmed))
//                .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.MapFrom(src => src.PhoneNumberConfirmed))
//                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImageUrl))
//                .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore()) // Ignore Identity-specific properties
//                .ForMember(dest => dest.AccessFailedCount, opt => opt.Ignore())
//                .ForMember(dest => dest.LockoutEnabled, opt => opt.Ignore())
//                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
//                .ForMember(dest => dest.NormalizedUserName, opt => opt.Ignore())
//                .ForMember(dest => dest.NormalizedEmail, opt => opt.Ignore())
//                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
//                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore());

//            // User mappings: Infrastructure to Domain
//            CreateMap<InfraUser, Domain.Entities.User>()
//                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
//                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => Email.Create(src.Email)))
//                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber != null ? PhoneNumber.Create(src.PhoneNumber) : null))
//                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
//                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.LastModifiedAt))
//                .ForMember(dest => dest.LastLoginAt, opt => opt.MapFrom(src => src.LastLoginAt))
//                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
//                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => src.EmailConfirmed))
//                .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.MapFrom(src => src.PhoneNumberConfirmed))
//                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImageUrl));

//            // Role mappings: Domain to Infrastructure
//            CreateMap<Domain.Entities.Role, InfraRole>()
//                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
//                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
//                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
//                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
//                .ForMember(dest => dest.LastModifiedAt, opt => opt.MapFrom(src => src.UpdatedAt))
//                .ForMember(dest => dest.NormalizedName, opt => opt.Ignore()); // Ignore Identity-specific

//            // Role mappings: Infrastructure to Domain
//            CreateMap<InfraRole, Domain.Entities.Role>()
//                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.LastModifiedAt));

//            // DTO mappings
//            CreateMap<Domain.Entities.User, UserDetailResponseDto>();
//            CreateMap<Domain.Entities.User, UserSummaryResponseDto>();
//            CreateMap<Domain.Entities.User, UserProfileResponseDto>();
//            CreateMap<Domain.Entities.Role, RoleDetailResponseDto>();
//            CreateMap<Domain.Entities.Role, RoleSummaryResponseDto>();
//        }
//    }
//}