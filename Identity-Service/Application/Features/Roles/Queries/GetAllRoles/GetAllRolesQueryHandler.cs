using AutoMapper;
using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using Identity_Service.Presentation.Dtos.Responses.Roles;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Identity_Service.Application.Features.Roles.Queries.GetAllRoles
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, List<RoleSummaryResponseDto>>
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public GetAllRolesQueryHandler(IIdentityService identityService, IMapper mapper)
        {
            _identityService = identityService;
            _mapper = mapper;
        }

        public async Task<List<RoleSummaryResponseDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _identityService.GetAllRolesAsync();

            var orderedRoles = roles.OrderBy(r => r.Name).ToList();

            return _mapper.Map<List<RoleSummaryResponseDto>>(orderedRoles);
        }
    }
}