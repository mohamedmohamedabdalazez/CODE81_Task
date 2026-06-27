namespace LibraryMS.Application.Common;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public int StatusCode { get; set; }

    public static ServiceResult<T> Ok(T data, string message = "Success") =>
        new() { Success = true, Message = message, Data = data, StatusCode = 200 };

    public static ServiceResult<T> Created(T data, string message = "Created") =>
        new() { Success = true, Message = message, Data = data, StatusCode = 201 };

    public static ServiceResult<T> Fail(string message, int statusCode = 400) =>
        new() { Success = false, Message = message, StatusCode = statusCode };

    public static ServiceResult<T> NotFound(string message) =>
        new() { Success = false, Message = message, StatusCode = 404 };

    public static ServiceResult<T> Conflict(string message) =>
        new() { Success = false, Message = message, StatusCode = 409 };
}
