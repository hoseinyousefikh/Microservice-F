using System.ComponentModel.DataAnnotations;

namespace Identity_Service.Presentation.Dtos.Requests.Admin
{
    public class UserFilterRequestDto
    {
        public string? SearchTerm { get; set; }

        [Range(1, int.MaxValue)]
        public int? Status { get; set; }

        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
    }
}