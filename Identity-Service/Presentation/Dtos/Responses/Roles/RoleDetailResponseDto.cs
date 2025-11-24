using System;

namespace Identity_Service.Presentation.Dtos.Responses.Roles
{
    public class RoleDetailResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public List<string> Permissions { get; set; } = new();
    }
}