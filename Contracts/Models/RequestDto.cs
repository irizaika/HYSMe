using Contracts.Enums;

namespace Contracts.Models
{
    public class RequestDto
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; } = string.Empty;
        public object Data { get; set; } = new object();
        public string AccessToken { get; set; } = string.Empty;

    }
}
