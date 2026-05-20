namespace Contracts.Models
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<Error>? Errors { get; set; }

        public static ApiResponse<T> Success(T? data = default, string? message = null)
            => new() { IsSuccess = true, Data = data, Message = message };

        public static ApiResponse<List<Error>> Fail(List<Error>? errors = default, string? message = null)
            => new() { IsSuccess = false, Errors = errors, Message = message };
    }
}
