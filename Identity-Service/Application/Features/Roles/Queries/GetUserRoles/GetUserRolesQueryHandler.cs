using AutoMapper;
using Identity_Service.Application.Common.Exceptions;
using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using Identity_Service.Presentation.Dtos.Responses.Roles;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Identity_Service.Application.Features.Roles.Queries.GetUserRoles
{
    public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, List<RoleSummaryResponseDto>>
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public GetUserRolesQueryHandler(IIdentityService identityService, IMapper mapper)
        {
            _identityService = identityService;
            _mapper = mapper;
        }

        public async Task<List<RoleSummaryResponseDto>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            var user = await _identityService.FindByIdAsync(request.UserId);
            if (user == null)
                throw new NotFoundException("User not found");

            var roleNames = await _identityService.GetRolesAsync(user);
            var roleEntities = new List<Domain.Entities.Role>();

            foreach (var roleName in roleNames)
            {
                var role = await _identityService.FindRoleByNameAsync(roleName);
                if (role != null)
                {
                    roleEntities.Add(role);
                }
            }

            return _mapper.Map<List<RoleSummaryResponseDto>>(roleEntities);
        }
    }
}