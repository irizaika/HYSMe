using Contracts.Models;

namespace WebApp.Services.Interfaces
{
    public interface IBaseService
    {
        Task<ApiResponse<T>?> SendAsync<T>(RequestDto requestDto, bool withBearer = true);
    }
}
