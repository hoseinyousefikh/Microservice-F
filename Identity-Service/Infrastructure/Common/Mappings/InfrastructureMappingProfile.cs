using AutoMapper;
using Identity_Service.Domain.Entities;
using Identity_Service.Domain.Entities.ValueObjects;
using InfraUser = Identity_Service.Infrastructure.Persistence.Entities.User;
using InfraRole = Identity_Service.Infrastructure.Persistence.Entities.Role;

namespace Identity_Service.Infrastructure.Common.Mappings
{
    public class InfrastructureMappingProfile : Profile
    {
        public InfrastructureMappingProfile()
        {
            // --- User Mappings (Domain <-> Infrastructure) ---

            // Domain to Infrastructure
            CreateMap<Domain.Entities.User, InfraUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.NormalizedUserName, opt => opt.MapFrom(src => src.Username.ToUpper()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
                .ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(src => src.Email.Value.ToUpper()))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber != null ? src.PhoneNumber.Value : null))
                .ForMember(dest => dest.LastModifiedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
                .ForMember(dest => dest.AccessFailedCount, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore());

            // Infrastructure to Domain (این بخش را اضافه یا اصلاح کن)
            CreateMap<InfraUser, Domain.Entities.User>()
                .ConstructUsing(src => new Domain.Entities.User(
                    src.UserName,
                    Email.Create(src.Email), // ایمیل را از string به ValueObject تبدیل کن
                    null, // PasswordHash در اینجا نیاز نیست
                    src.FirstName,
                    src.LastName,
                    src.PhoneNumber != null ? PhoneNumber.Create(src.PhoneNumber) : null)) // شماره تلفن را از string به ValueObject تبدیل کن
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status)) // فرض می‌کنیم enum ها یکسان هستند
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.LastModifiedAt))
                .ForMember(dest => dest.LastLoginAt, opt => opt.MapFrom(src => src.LastLoginAt))
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImageUrl))
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => src.EmailConfirmed))
                .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.MapFrom(src => src.PhoneNumberConfirmed))
                // نادیده گرفتن فیلدهایی که در Domain توسط constructor مدیریت می‌شوند
                .ForMember(dest => dest.Username, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore());

            // --- Role Mappings (Domain <-> Infrastructure) ---
            // (این بخش بدون تغییر باقی می‌ماند)
            CreateMap<Domain.Entities.Role, InfraRole>()
                .ForMember(dest => dest.NormalizedName, opt => opt.MapFrom(src => src.Name.ToUpper()))
                .ForMember(dest => dest.LastModifiedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore());

            CreateMap<InfraRole, Domain.Entities.Role>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.LastModifiedAt));
        }
    }
}