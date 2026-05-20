using Contracts.Models;
using Contracts.Enums;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using WebApp.Services.Interfaces;
using WebApp.Models;

namespace WebApp.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;
        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        public async Task<ApiResponse<T>?> SendAsync<T>(RequestDto requestDto, bool withBearer = true)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("HYSMeApi");

                var message = new HttpRequestMessage
                {
                    RequestUri = new Uri(requestDto.Url),
                    Method = requestDto.ApiType switch
                    {
                        ApiType.POST => HttpMethod.Post,
                        ApiType.PUT => HttpMethod.Put,
                        ApiType.DELETE => HttpMethod.Delete,
                        _ => HttpMethod.Get
                    }
                };

                message.Headers.Add("Accept", "application/json");

                if (withBearer)
                {
                    var token = _tokenProvider.GetToken();
                    message.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                if (requestDto.Data != null)
                {
                    message.Content = new StringContent(
                        JsonConvert.SerializeObject(requestDto.Data),
                        Encoding.UTF8,
                        "application/json");
                }

                var response = await client.SendAsync(message);

                var content = await response.Content.ReadAsStringAsync();

                // try to deserialize ApiResponse<T>
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(content);

                if (apiResponse == null)
                {
                    return new ApiResponse<T>
                    {
                        IsSuccess = false,
                        Message = "Failed to parse response"
                    };
                }

                // Override success flag if HTTP failed
                if (!response.IsSuccessStatusCode)
                {
                    apiResponse.IsSuccess = false;
                }

                return apiResponse;

            }
            catch (Exception ex)
            {
                

                return new ApiResponse<T>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
