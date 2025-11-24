using System.ComponentModel.DataAnnotations;

namespace Identity_Service.Presentation.Dtos.Requests.Authentication
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}