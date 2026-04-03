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
        public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("HYSMeApi");
                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");

                if (withBearer)
                {
                    var token = _tokenProvider.GetToken();
                    message.Headers.Add("Authorization", $"Bearer {token}");
                }

                message.RequestUri = new Uri(requestDto.Url);

                if (requestDto.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
                }

                HttpResponseMessage? apiResponse = null;

                message.Method = requestDto.ApiType switch
                {
                    ApiType.POST => HttpMethod.Post,
                    ApiType.PUT => HttpMethod.Put,
                    ApiType.DELETE => HttpMethod.Delete,
                    _ => HttpMethod.Get,
                };

                apiResponse = await client.SendAsync(message);

                if (apiResponse == null)
                {
                    return new ResponseDto() { IsSuccess = false, Message = "Something went wrong" };
                } 
                else if(!apiResponse.IsSuccessStatusCode)
                {

                    switch (apiResponse.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            return new() { IsSuccess = false, Message = "Not Found" };
                        case HttpStatusCode.Forbidden:
                            return new() { IsSuccess = false, Message = "Access Denied" };
                        case HttpStatusCode.Unauthorized:
                            return new() { IsSuccess = false, Message = "Unauthorized" };
                        case HttpStatusCode.InternalServerError:
                            return new() { IsSuccess = false, Message = "Internal Server Error" };
                        case HttpStatusCode.BadRequest:
                            var apiContent = await apiResponse.Content.ReadAsStringAsync();
                            var validationError = JsonConvert.DeserializeObject<ValidationErrorResponse>(apiContent);

                            var errorMessage = string.Join(" | ",
                                validationError?.Errors?.SelectMany(e => e.Value.Select(v => $"{e.Key}: {v}")) ?? ["Bad request"]);

                            return new()
                            {
                                IsSuccess = false,
                                Message = errorMessage ?? ""
                            };
                        default:
                            return new() { IsSuccess = false, Message = "Some error accured" };
                    }
                }
                else
                {
                    var apiContent = await apiResponse.Content.ReadAsStringAsync();
                    var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                    return apiResponseDto;
                }
            }
            catch (Exception ex)
            {
                var dto = new ResponseDto
                {
                    Message = ex.Message.ToString(),
                    IsSuccess = false
                };
                return dto;
            }
        }
    }
}
