namespace LibraryMS.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    public static ApiResponse<T> From(ServiceResult<T> result) => new()
    {
        Success = result.Success,
        Message = result.Message,
        Data = result.Data
    };

    public static ApiResponse<T> Error(string message, List<string>? errors = null) => new()
    {
        Success = false,
        Message = message,
        Errors = errors
    };
}
