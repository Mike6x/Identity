namespace Identity.Core.Dtos;

public class ErrorResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string ErrorCode { get; set; }
}
