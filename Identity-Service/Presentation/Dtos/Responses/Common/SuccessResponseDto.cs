namespace Identity_Service.Presentation.Dtos.Responses.Common
{
    public class SuccessResponseDto
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "Operation completed successfully.";
        public object? Data { get; set; }
    }
}