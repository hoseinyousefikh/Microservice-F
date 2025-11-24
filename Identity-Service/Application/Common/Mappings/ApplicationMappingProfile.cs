using AutoMapper;
using Identity_Service.Presentation.Dtos.Responses.Roles;
using Identity_Service.Presentation.Dtos.Responses.UserManagement;

namespace Identity_Service.Application.Common.Mappings
{
    public class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {
            // --- User Mappings (Domain to DTOs) ---

            CreateMap<Domain.Entities.User, UserProfileResponseDto>();

            CreateMap<Domain.Entities.User, UserDetailResponseDto>();

            CreateMap<Domain.Entities.User, UserSummaryResponseDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Id)); // تبدیل enum به int برای خروجی

            // --- Role Mappings (Domain to DTOs) ---

            CreateMap<Domain.Entities.Role, RoleDetailResponseDto>();

            CreateMap<Domain.Entities.Role, RoleSummaryResponseDto>();
        }
    }
}
