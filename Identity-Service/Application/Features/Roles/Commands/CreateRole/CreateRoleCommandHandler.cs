using AutoMapper;
using Identity_Service.Application.Common.Exceptions;
using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using Identity_Service.Presentation.Dtos.Responses.Roles;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Identity_Service.Application.Features.Roles.Commands.CreateRole
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleDetailResponseDto>
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public CreateRoleCommandHandler(IIdentityService identityService, IMapper mapper)
        {
            _identityService = identityService;
            _mapper = mapper;
        }

        public async Task<RoleDetailResponseDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            if (await _identityService.FindRoleByNameAsync(request.Name) != null)
                throw new BadRequestException("Role already exists");

            var role = new Role(request.Name, request.Description);
            var result = await _identityService.CreateRoleAsync(role);

            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

            return _mapper.Map<RoleDetailResponseDto>(role);
        }
    }
}