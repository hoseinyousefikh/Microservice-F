using System;

namespace Identity_Service.Presentation.Dtos.Responses.Roles
{
    public class RoleSummaryResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}